"""Refine Scenario mattes against original pixels; no additional generation.
Run after PrepareAssets, before FinishAssets publishes runtime textures.
"""
from PIL import Image, ImageDraw
from pathlib import Path
import numpy as np, cv2
b=Path(__file__).resolve().parent
p=b.parent/'Assets'
src=Image.open(b/'OriginalReference.png').convert('RGBA')
def mask(size,draw):
    m=Image.new('L',(size[0]*4,size[1]*4));d=ImageDraw.Draw(m)
    class Draw:
        def ellipse(self,box):d.ellipse(tuple(round(v*4) for v in box),fill=255)
        def polygon(self,pts):d.polygon([(round(x*4),round(y*4)) for x,y in pts],fill=255)
    draw(Draw());return m.resize(size,Image.Resampling.LANCZOS)
# Remove the neighbouring moves crest from the goal panel while preserving continuous rails.
# Row colours come from an unobstructed source strip at the same vertical position.
a=Image.open(p/'goals.png').convert('RGBA');arr=np.array(a);original=np.array(src)
rows=np.median(original[680:724,330:355,:3],axis=1)
for y in range(298,342):
    for x in range(145,386):
        edge=min((x-145)/16,(385-x)/16,(y-298)/5,1)
        t=max(0,min(1,edge));arr[y,x,:3]=(arr[y,x,:3]*(1-t)+rows[y-298]*t).astype(np.uint8)
        arr[y,x,3]=255
arr[342:,145:386,3]=0
Image.fromarray(arr).save(p/'goals.png')
# A tight circular silhouette omits neighbouring panel/scenery wedges. The four original tips survive.
a=Image.open(p/'moves.png').convert('RGBA')
def moves(d):
    d.ellipse((14,17,200,194))
    d.polygon([(89,23),(106,3),(122,21)])
    d.polygon([(13,88),(3,104),(24,116)])
    d.polygon([(194,85),(210,102),(190,117)])
    d.polygon([(87,186),(107,204),(126,183)])
a.putalpha(mask(a.size,moves));a.save(p/'moves.png')
# Original full medallions: the previous intersection of two masks removed rim pixels.
# Restore complete source pixels using a supersampled contour, including the stock badge.
for n,x,cx,rx,bx in [('mace',147,90,77,133),('firebird',307,89,77,133),('shield',466,88,76,124),('potion',623,87,75,123)]:
    a=src.crop((x,925,x+180,1105))
    def shape(d):
        d.ellipse((cx-rx,13,cx+rx,160))
        d.polygon([(cx-14,17),(cx,3),(cx+14,17)])
        d.polygon([(cx-14,155),(cx,172),(cx+14,154)])
        d.polygon([(cx-rx+5,73),(cx-rx-8,86),(cx-rx+6,98)])
        d.polygon([(cx+rx-5,73),(cx+rx+8,86),(cx+rx-6,98)])
        d.ellipse((bx-27,114,bx+27,168))
    a.putalpha(mask(a.size,shape))
    # Native live badges cover the reference counts entirely.
    pixels=np.array(a);hole=np.zeros(pixels.shape[:2],np.uint8)
    cv2.circle(hole,(bx,141),19,255,-1)
    pixels[:,:,:3]=cv2.inpaint(pixels[:,:,:3],hole,3,cv2.INPAINT_TELEA)
    Image.fromarray(pixels).save(p/(n+'.png'))
# Reconstruct the missing top closure using the original thin gold rail.
a=Image.open(p/'rewards.png').convert('RGBA');al=a.getchannel('A');d=ImageDraw.Draw(al)
# Neighbouring booster pixels above the open reward frame are not part of this asset.
d.rectangle((0,0,697,5),fill=0);d.rectangle((116,0,581,12),fill=0)
# Same-source rail bridges the two original corner tails without introducing a new ornament.
rail=src.crop((265,1446,400,1452)).resize((466,6),Image.Resampling.BICUBIC)
a.paste(rail,(116,13));d.rectangle((116,13,581,25),fill=255)
a.putalpha(al);a.save(p/'rewards.png')
