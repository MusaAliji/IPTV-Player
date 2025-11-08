import React, { useMemo } from 'react';
import type { EPGProgram, Channel } from '@iptv-player/shared-types';
import { formatEPGTime } from '@iptv-player/shared-types';
import './EPGGrid.css';

interface EPGGridProps {
  channels: Channel[];
  programs: EPGProgram[];
  startTime: Date;
  endTime: Date;
  onProgramClick?: (program: EPGProgram) => void;
}

const EPGGrid: React.FC<EPGGridProps> = ({
  channels,
  programs,
  startTime,
  endTime,
  onProgramClick,
}) => {
  const timeSlots = useMemo(() => {
    const slots: Date[] = [];
    const current = new Date(startTime);
    const end = new Date(endTime);

    while (current <= end) {
      slots.push(new Date(current));
      current.setMinutes(current.getMinutes() + 30);
    }

    return slots;
  }, [startTime, endTime]);

  const getProgramsForChannel = (channelId: number) => {
    return programs.filter((p) => p.channelId === channelId);
  };

  const getProgramWidth = (program: EPGProgram) => {
    const start = new Date(program.startTime).getTime();
    const end = new Date(program.endTime).getTime();
    const totalDuration = endTime.getTime() - startTime.getTime();
    const programDuration = end - start;

    return (programDuration / totalDuration) * 100;
  };

  const getProgramOffset = (program: EPGProgram) => {
    const start = new Date(program.startTime).getTime();
    const gridStart = startTime.getTime();
    const totalDuration = endTime.getTime() - startTime.getTime();
    const offset = start - gridStart;

    return (offset / totalDuration) * 100;
  };

  return (
    <div className="epg-grid">
      <div className="epg-header">
        <div className="channel-column-header">Channels</div>
        <div className="timeline">
          {timeSlots.map((slot, index) => (
            <div key={index} className="time-slot">
              {formatEPGTime(slot.toISOString())}
            </div>
          ))}
        </div>
      </div>

      <div className="epg-content">
        {channels.map((channel) => {
          const channelPrograms = getProgramsForChannel(channel.id);

          return (
            <div key={channel.id} className="epg-row">
              <div className="channel-info">
                <div className="channel-number">{channel.channelNumber}</div>
                <div className="channel-name">{channel.name}</div>
              </div>

              <div className="programs-timeline">
                {channelPrograms.map((program) => (
                  <div
                    key={program.id}
                    className="program-block"
                    style={{
                      left: `${getProgramOffset(program)}%`,
                      width: `${getProgramWidth(program)}%`,
                    }}
                    onClick={() => onProgramClick?.(program)}
                  >
                    <div className="program-title">{program.title}</div>
                    <div className="program-time">
                      {formatEPGTime(program.startTime)} - {formatEPGTime(program.endTime)}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default EPGGrid;
