#!/bin/bash

cd /home/user/IPTV-Player/web/src

# Fix unused imports
sed -i 's/import type { EPGProgram, Channel }/import type { EPGProgram }/g' pages/epg/EPGPage.tsx
sed -i 's/import type { Channel, Content }/import type { Channel }/g' pages/player/PlayerPage.tsx

# Fix AxiosInstance import
sed -i 's/import axios, { AxiosInstance }/import axios, type { AxiosInstance }/g' services/api.ts

# Remove unused API_ENDPOINTS import
sed -i 's/, API_ENDPOINTS//g' services/api.ts
sed -i '/^import { API_ENDPOINTS } from/d' services/api.ts

# Fix LibraryPage - replace remaining page/pageSize references
sed -i 's/page: 1, pageSize:/limit:/g' pages/library/LibraryPage.tsx
sed -i 's/pageSize:/limit:/g' pages/library/LibraryPage.tsx

echo "Final fixes applied"
