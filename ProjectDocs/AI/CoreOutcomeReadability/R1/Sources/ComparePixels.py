"""Decode the unproven core-page pairs with local libpng; inspect pixels, never edit images."""
from pathlib import Path
import ctypes,ctypes.util,json
BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
lib=ctypes.CDLL(ctypes.util.find_library('png16'))
class PngImage(ctypes.Structure):
    _fields_=[('opaque',ctypes.c_void_p),('version',ctypes.c_uint32),('width',ctypes.c_uint32),('height',ctypes.c_uint32),('format',ctypes.c_uint32),('flags',ctypes.c_uint32),('colormap_entries',ctypes.c_uint32),('warning_or_error',ctypes.c_uint32),('message',ctypes.c_char*64)]
lib.png_image_begin_read_from_file.argtypes=[ctypes.POINTER(PngImage),ctypes.c_char_p];lib.png_image_begin_read_from_file.restype=ctypes.c_int
lib.png_image_finish_read.argtypes=[ctypes.POINTER(PngImage),ctypes.c_void_p,ctypes.c_void_p,ctypes.c_int32,ctypes.c_void_p];lib.png_image_finish_read.restype=ctypes.c_int
lib.png_image_free.argtypes=[ctypes.POINTER(PngImage)]
def decode(path):
    im=PngImage();im.version=1
    assert lib.png_image_begin_read_from_file(ctypes.byref(im),str(path).encode())==1,im.message
    im.format=3 # PNG_FORMAT_RGBA, 8-bit destination.
    w,h=im.width,im.height;buf=(ctypes.c_ubyte*(w*h*4))()
    assert lib.png_image_finish_read(ctypes.byref(im),None,buf,0,None)==1,im.message
    lib.png_image_free(ctypes.byref(im));return w,h,bytes(buf)
comparison=json.loads((QA/'CaptureComparison.json').read_text());rows=[]
for row in comparison['rows']:
    if row['pixelIdentityProven']:continue
    rel=row['path'];w,h,a=decode(QA/'Baseline'/rel);w2,h2,b=decode(QA/'PlayFinal'/rel);assert (w,h)==(w2,h2)
    differences=[i for i,(x,y) in enumerate(zip(a,b)) if x!=y];pixels={i//4 for i in differences}
    box=[min(i%w for i in pixels),min(i//w for i in pixels),max(i%w for i in pixels),max(i//w for i in pixels)] if pixels else None
    result={'path':rel,'decodedPixelIdentical':not pixels,'changedPixels':len(pixels),'pixelCount':w*h,'changedPixelPercent':100*len(pixels)/(w*h),'changedChannels':len(differences),'maxChannelDifference':max((abs(a[i]-b[i]) for i in differences),default=0),'boundingBoxInclusiveTopLeft':box,'beforeAndAfterLayoutJsonIdentical':json.loads((QA/'Baseline'/Path(rel).with_suffix('.json')).read_text())==json.loads((QA/'PlayFinal'/Path(rel).with_suffix('.json')).read_text())}
    rows.append(result)
(QA/'MapPixelComparison.json').write_text(json.dumps({'method':'Local libpng simplified API RGBA8 decode, byte comparison; no image edits','physicalDeviceComparison':False,'rows':rows},indent=2)+'\n')
print(json.dumps(rows,indent=2))
