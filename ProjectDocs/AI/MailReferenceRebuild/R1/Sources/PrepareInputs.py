from pathlib import Path
from PIL import Image,ImageDraw
import json
b=Path(__file__).resolve().parent;im=Image.open(b/'Inbox.png').convert('RGB');sheet=Image.new('RGB',(1800,720),(230,230,230))
# Keep the original thin row rails/corners; only replace the occupied inner face.
row=im.crop((49,492,894,621));donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/table_panel_r4.png').convert('RGB');donor=donor.crop((donor.width//3,donor.height//3,donor.width*2//3,donor.height*2//3));face=donor.resize((825,109),Image.Resampling.LANCZOS);row.paste(face,(10,10));row=row.resize((1690,258),Image.Resampling.LANCZOS);sheet.paste(row,(50,35))
footer=im.crop((277,1480,670,1618)).resize((786,276),Image.Resampling.LANCZOS);sheet.paste(footer,(500,380))
sheet.save(b/'Sheet.png');(b/'Bounds.json').write_text(json.dumps({'row_rebuild':[50,35,1740,293],'claim_all_ru_rebuild':[500,380,1286,656],'rowSource':[49,492,894,621],'footerSource':[277,1480,670,1618],'rowInteriorRepair':[10,10,835,119],'faceDonor':'TeamRankingAligned/table_panel_r4.png central navy'},indent=2))
