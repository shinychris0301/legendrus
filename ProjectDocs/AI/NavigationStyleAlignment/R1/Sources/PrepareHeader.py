from pathlib import Path
from PIL import Image
b=Path('ProjectDocs/AI/NavigationStyleAlignment/R1/Sources')
im=Image.open(b/'Scenario/header_alpha/booster_medallion_trial01.png').convert('RGBA')
im.crop(im.getbbox()).save('Assets/LegendsOfRus/Resources/ProductionUI/NavigationAligned/header.png')
