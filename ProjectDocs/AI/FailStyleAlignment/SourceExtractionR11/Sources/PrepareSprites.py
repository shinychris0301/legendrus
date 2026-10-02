"""Scenario alpha + original RGB. Local masked texture repair removes baked labels.
No generated redesign pixels enter runtime. Inspect Staging/ and QA before install.
"""
from pathlib import Path
import json,hashlib
import numpy as np
import cv2
from PIL import Image,ImageDraw
B=Path(__file__).resolve().parents[1];S=B/'Sources';OUT=B/'Staging';OUT.mkdir(exist_ok=True)
entries=json.loads((S/'UiRepairMap.json').read_text());report=[]
for e in entries:
    n=e['name'];src=np.array(Image.open(S/'Crops'/f'{n}.png').convert('RGB'));h,w=src.shape[:2]
    p=S/'Scenario'/('TitleAlpha' if n=='title' else 'Alpha_'+n)/'booster_medallion_trial01.png'
    if not p.exists():continue
    alpha=np.array(Image.open(p).convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
    allowed=np.zeros((h,w),np.uint8)
    for x0,y0,x1,y1 in e['editBoxes']:allowed[y0:y1,x0:x1]=255
    # Original texture donors avoid dark glyph shadows and long inpaint streaks.
    donors={
        'title':[(37,111,85,201),(37,111,85,201)],
        'retry':[(65,34,103,78)],'paid':[(33,29,64,68),(213,96,242,125)],
        'ad':[(263,32,292,77)],'nameplate':[(7,18,17,38)],
        'heading':[(25,13,70,37)]}
    if n=='ad':
        e['editBoxes']=[[59,20,264,80]]
    if n=='title':e['editBoxes']=[[91,92,612,169],[132,167,573,216]]
    allowed[:]=0
    for x0,y0,x1,y1 in e['editBoxes']:allowed[y0:y1,x0:x1]=255
    fixed=src.copy();mask=allowed.copy()
    for i,(x0,y0,x1,y1) in enumerate(e['editBoxes']):
        if n=='goals':continue
        if n in donors:
            dx0,dy0,dx1,dy1=donors[n][i];tile=src[dy0:dy1,dx0:dx1]
        else:
            tile=src[150:158,120:125]
        if n in ['title','nameplate']:
            clean=np.array(Image.open(S/'Crops/goals.png').convert('RGB'))
            tile=clean[32:115,157:235]
        if n in ['mace','firebird','shield','potion']:
            # Fit the original green badge surface around the removed glyph, with
            # gold rim and ivory lettering excluded from the sample set.
            yy,xx=np.mgrid[:h,:w];rr,gg,bb=src.astype(float).transpose(2,0,1)
            cx=(x0+x1)/2;cy=(y0+y1)/2
            valid=((xx-cx)**2+(yy-cy)**2<21**2)&(allowed==0)&(gg>rr*1.2)&(gg>bb*1.5)&(gg>20)
            coords=np.stack([np.ones_like(xx),xx-cx,yy-cy],axis=-1).astype(float)
            if valid.sum()>8:
                coefficients=np.linalg.lstsq(coords[valid],src[valid].astype(float),rcond=None)[0]
                fill=np.clip(coords@coefficients,0,255).astype(np.uint8)
                circle=np.zeros((h,w),np.uint8)
                center=(145,153) if n=='potion' else (int(cx+2),153)
                cv2.ellipse(circle,center,(22,22) if n=='potion' else (19,20),0,0,360,255,-1)
                allowed=np.maximum(allowed,circle);mask=allowed.copy()
                fixed[allowed>0]=fill[allowed>0]
                continue
        tile=np.concatenate([tile,tile[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0)
        hh,ww=y1-y0,x1-x0
        donor=np.tile(tile,(hh//tile.shape[0]+1,ww//tile.shape[1]+1,1))[:hh,:ww]
        yy,xx=np.mgrid[:hh,:ww];dist=np.minimum.reduce([xx,yy,ww-1-xx,hh-1-yy])
        weight=np.clip(dist/3,0,1)[:,:,None]
        fixed[y0:y1,x0:x1]=np.round(src[y0:y1,x0:x1]*(1-weight)+donor*weight).astype(np.uint8)
    if n=='goals':
        # Empty navy source texture between the first and second goal, mirrored to
        # avoid hard repeat seams. Only replace the explicitly recorded inset.
        tile=src[32:115,157:235];tile=np.concatenate([tile,tile[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0)
        donor=np.tile(tile,(2,5,1))[:h,:w]
        area=np.zeros((h,w),np.uint8);area[13:160,42:557]=255
        weight=cv2.GaussianBlur(area,(11,11),2).astype(float)/255;weight[allowed==0]=0
        fixed=np.round(src*(1-weight[:,:,None])+donor*weight[:,:,None]).astype(np.uint8);mask=area
    # Poisson blend each repair against the untouched surface; no rectangular seams.
    if n!='goals':
        donor=fixed.copy();composite=src.copy()
        contours,_=cv2.findContours(allowed,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)
        for contour in contours:
            x,y,ww,hh=cv2.boundingRect(contour)
            part=np.zeros_like(allowed);cv2.drawContours(part,[contour],-1,255,-1)
            blended=cv2.seamlessClone(donor,src,part.copy(),(x+ww//2,y+hh//2),cv2.NORMAL_CLONE)
            composite[part>0]=blended[part>0]
        fixed=composite
    # Pixa removes the very dark heading face; explicitly retain its opaque UI
    # surface. Reject disconnected neighbouring ornament fragments in other ROIs.
    if n=='heading':
        alpha=np.zeros((h,w),np.uint8)
        cv2.fillPoly(alpha,[np.array([(20,3),(471,3),(488,24),(471,45),(20,45),(3,24)])],255)
    if n=='nameplate':alpha[:,184:]=0;alpha[49:]=0
    if n in ['mace','firebird','shield','potion']:alpha[h-4:]=0
    binary=(alpha>30).astype('uint8')
    count,labels,stats,centroids=cv2.connectedComponentsWithStats(binary)
    if count>1:
        largest=1+int(np.argmax(stats[1:,cv2.CC_STAT_AREA]));keep=(labels==largest).astype('uint8')
        keep=cv2.dilate(keep,np.ones((3,3),np.uint8));alpha[keep==0]=0
    # Restore every protected source RGB byte, including opaque panel interiors.
    fixed[allowed==0]=src[allowed==0]
    rgba=np.dstack([fixed,alpha]);Image.fromarray(rgba).save(OUT/f'{n}_r11.png')
    Image.fromarray(mask).save(OUT/f'{n}_repair_mask.png')
    Image.fromarray(allowed).save(OUT/f'{n}_allowed_mask.png')
    changed=np.any(fixed!=src,axis=2)
    report.append({'name':n,'sourceBox':e['sourceBox'],'size':[w,h],'scenarioAlphaSize':Image.open(p).size,'changedRgbPixels':int(changed.sum()),'changedOutsideRepairRegion':int((changed&(allowed==0)).sum()),'transparentPixels':int((alpha==0).sum()),'opaquePixels':int((alpha==255).sum()),'sourceAlpha':str(p.relative_to(B)),'repair':'Local original-surface texture cloning / fitted badge face, bounded Poisson seam blending; recorded repair-mask PNG','sha256':hashlib.sha256((OUT/f'{n}_r11.png').read_bytes()).hexdigest()})
(B/'QA/ExtractionAudit.json').write_text(json.dumps(report,indent=2)+'\n')
# Diagnostic contact sheet only; the implementation/review remains one portrait.
canvas=Image.new('RGB',(1100,2400),'#555555');d=ImageDraw.Draw(canvas);y=4
for rec in report:
    im=Image.open(OUT/(rec['name']+'_r11.png'));scale=min(1.0,1000/im.width);im=im.resize((int(im.width*scale),int(im.height*scale)))
    if y+im.height+24>canvas.height:break
    d.text((6,y),rec['name'],fill='white');y+=16
    canvas.paste(im,(10,y),im);y+=im.height+12
canvas.crop((0,0,1100,min(y,2400))).save(B/'QA/PreparedSprites.png')
print('Prepared',len(report),'staging sprites; production untouched.')
