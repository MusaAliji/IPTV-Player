#!/bin/bash

# Fix type imports in component files
cd /home/user/IPTV-Player/web/src

# Fix ChannelList.tsx
sed -i "s/import { Channel }/import type { Channel }/g" components/channel/ChannelList.tsx

# Fix ContentCard.tsx
sed -i "s/import { Content }/import type { Content }/g" components/content/ContentCard.tsx

# Fix EPGGrid.tsx
sed -i "s/import { EPGProgram, Channel }/import type { EPGProgram, Channel }/g" components/epg/EPGGrid.tsx

# Fix pages
sed -i "s/import { Content, Channel }/import type { Content, Channel }/g" pages/home/HomePage.tsx
sed -i "s/import { EPGProgram, Channel }/import type { EPGProgram, Channel }/g" pages/epg/EPGPage.tsx
sed -i "s/import { Content, ContentType }/import type { Content } from '@iptv-player\/shared-types';\nimport { ContentType }/g" pages/library/LibraryPage.tsx
sed -i "s/import { Channel, Content }/import type { Channel, Content }/g" pages/player/PlayerPage.tsx

echo "Fixed type imports"
