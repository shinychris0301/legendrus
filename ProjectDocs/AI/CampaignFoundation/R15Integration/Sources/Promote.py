"""Reproduce the explicitly authorized R14 content integration; preserve existing metas."""
from pathlib import Path
import shutil
root=Path(__file__).resolve().parents[5]
source=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R14'
target=root/'Assets/LegendsOfRus/Resources/CampaignActive'
for name in ['levels','story','story_ru','story_en']:
    shutil.copyfile(source/(name+'.json'),target/(name+'.json'))
shutil.copyfile(root/'ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json',target/'definition.json')
