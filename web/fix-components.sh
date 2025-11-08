#!/bin/bash

cd /home/user/IPTV-Player/web/src

# Fix ChannelList - channel.number should be channel.channelNumber
sed -i 's/channel\.number}/channel.channelNumber}/g' components/channel/ChannelList.tsx
sed -i 's/{channel\.number}/{channel.channelNumber}/g' components/channel/ChannelList.tsx

# Remove channel.description references (doesn't exist on Channel type)
sed -i '/{channel\.description/,/}/d' components/channel/ChannelList.tsx

# Fix ContentCard - content.releaseYear should be releaseDate
sed -i 's/content\.releaseYear/content.releaseDate/g' components/content/ContentCard.tsx

# Fix EPGGrid - remove formatDuration import
sed -i 's/formatEPGTime, formatDuration/formatEPGTime/g' components/epg/EPGGrid.tsx
# Fix EPGGrid - channelId comparison (number vs string)
sed -i "s/(p) => p.channelId === channelId/(p) => p.channelId === channel.id/g" components/epg/EPGGrid.tsx
# Fix EPGGrid - channel.number to channel.channelNumber
sed -i 's/{channel\.number}/{channel.channelNumber}/g' components/epg/EPGGrid.tsx
# Fix EPGGrid - formatEPGTime expects string not Date
sed -i 's/formatEPGTime(slot)/formatEPGTime(slot.toISOString())/g' components/epg/EPGGrid.tsx
sed -i 's/formatEPGTime(new Date(program.startTime))/formatEPGTime(program.startTime)/g' components/epg/EPGGrid.tsx
sed -i 's/formatEPGTime(new Date(program.endTime))/formatEPGTime(program.endTime)/g' components/epg/EPGGrid.tsx

# Fix VideoPlayer - remove unused event parameter
sed -i 's/hls.on(Hls.Events.ERROR, (event, data)/hls.on(Hls.Events.ERROR, (_event, data)/g' components/video/VideoPlayer.tsx

# Fix pages to use limit/offset instead of page/pageSize
sed -i 's/{ page: 1, pageSize: 20 }/{ limit: 20, offset: 0 }/g' pages/home/HomePage.tsx
sed -i 's/{ page: 1, pageSize: 10 }/{ limit: 10, offset: 0 }/g' pages/home/HomePage.tsx
sed -i 's/{ page: 1, pageSize: 20 }/{ limit: 20, offset: 0 }/g' pages/epg/EPGPage.tsx
sed -i 's/{ page: 1, pageSize: 100 }/{ limit: 100, offset: 0 }/g' pages/library/LibraryPage.tsx
sed -i 's/{ page: 1, pageSize: 50 }/{ limit: 50, offset: 0 }/g' pages/player/PlayerPage.tsx

# Fix PlayerPage - remove channel.description references
sed -i '/{currentChannel\.description/,/}/d' pages/player/PlayerPage.tsx

# Fix HomePage - Remove unused 'contents' variable
sed -i 's/const { contents, channels,/const { channels,/g' pages/home/HomePage.tsx

# Fix EPGPage - Remove unused 'Channel' import
# (already done by previous script)

# Fix PlayerPage - Remove unused 'Content' import
# (already done by previous script)

# Fix PlayerPage - selectedChannelId type mismatch (number vs string)
sed -i 's/selectedChannelId={currentChannel?.id}/selectedChannelId={currentChannel?.id.toString()}/g' pages/player/PlayerPage.tsx

echo "Component fixes applied"
