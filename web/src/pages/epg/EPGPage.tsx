import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { EPGProgram } from '@muski/iptv-shared-types';
import { epgService } from '@services/epgService';
import { contentService } from '@services/contentService';
import { useEPGStore } from '@store/useEPGStore';
import { useContentStore } from '@store/useContentStore';
import EPGGrid from '@components/epg/EPGGrid';
import './EPGPage.css';

const EPGPage: React.FC = () => {
  const navigate = useNavigate();
  const { programs, setPrograms, setLoading, setError } = useEPGStore();
  const { channels, setChannels } = useContentStore();
  const [startTime, setStartTime] = useState<Date>(() => {
    const now = new Date();
    now.setHours(now.getHours() - 2);
    return now;
  });
  const [endTime, setEndTime] = useState<Date>(() => {
    const now = new Date();
    now.setHours(now.getHours() + 6);
    return now;
  });

  useEffect(() => {
    loadData();
  }, [startTime, endTime]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [channelResponse, programsData] = await Promise.all([
        contentService.getAllChannels({ limit: 20, offset: 0 }),
        epgService.getAllPrograms(startTime, endTime),
      ]);

      setChannels(channelResponse.items);
      setPrograms(programsData);
    } catch (error: any) {
      console.error('Failed to load EPG data:', error);
      setError(error.message || 'Failed to load EPG data');
    } finally {
      setLoading(false);
    }
  };

  const handleProgramClick = (program: EPGProgram) => {
    const channel = channels.find((c) => c.id === program.channelId);
    if (channel) {
      navigate(`/player?channelId=${channel.id}`);
    }
  };

  const handlePreviousTimeSlot = () => {
    const newStart = new Date(startTime);
    const newEnd = new Date(endTime);
    newStart.setHours(newStart.getHours() - 4);
    newEnd.setHours(newEnd.getHours() - 4);
    setStartTime(newStart);
    setEndTime(newEnd);
  };

  const handleNextTimeSlot = () => {
    const newStart = new Date(startTime);
    const newEnd = new Date(endTime);
    newStart.setHours(newStart.getHours() + 4);
    newEnd.setHours(newEnd.getHours() + 4);
    setStartTime(newStart);
    setEndTime(newEnd);
  };

  return (
    <div className="epg-page">
      <div className="epg-header">
        <h1>TV Guide</h1>
        <div className="epg-controls">
          <button onClick={handlePreviousTimeSlot}>Previous</button>
          <span>
            {startTime.toLocaleTimeString()} - {endTime.toLocaleTimeString()}
          </span>
          <button onClick={handleNextTimeSlot}>Next</button>
        </div>
      </div>

      {channels.length > 0 && programs.length > 0 && (
        <EPGGrid
          channels={channels}
          programs={programs}
          startTime={startTime}
          endTime={endTime}
          onProgramClick={handleProgramClick}
        />
      )}

      {channels.length === 0 && <p className="no-data">No channels available</p>}
    </div>
  );
};

export default EPGPage;
