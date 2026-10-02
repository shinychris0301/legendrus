from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
b=Path(__file__).resolve().parent
im=Image.open(b/'Inputs/outer_upper_left_r11.png').convert('RGB');mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
d.polygon([(0,0),(160,0),(151,20),(120,38),(116,70),(92,101),(76,103),(55,83),(50,41),(30,34),(15,22)],fill=255)
d.polygon([(181,0),(245,0),(245,122),(221,122),(209,107),(190,98),(171,77),(167,52),(177,34),(197,23)],fill=255)
d.polygon([(101,125),(245,125),(245,192),(80,192),(82,157),(90,137)],fill=255)
mask.save(b/'UpperLeftControlsMask.png');Image.composite(Image.new('RGB',im.size,'white'),im,mask).save(b/'Inputs/outer_upper_left_isolation_r11.png')
