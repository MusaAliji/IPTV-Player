/**
 * Formatting Utilities
 * Common formatting functions for dates, times, durations, etc.
 */

/**
 * Formats duration in seconds to HH:MM:SS or MM:SS format
 */
export function formatDuration(seconds: number): string {
  if (seconds < 0) return '00:00';

  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = Math.floor(seconds % 60);

  if (hours > 0) {
    return `${padZero(hours)}:${padZero(minutes)}:${padZero(secs)}`;
  }
  return `${padZero(minutes)}:${padZero(secs)}`;
}

/**
 * Formats duration in seconds to human-readable format (e.g., "1h 30m", "45m", "30s")
 */
export function formatDurationHuman(seconds: number): string {
  if (seconds < 0) return '0s';

  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = Math.floor(seconds % 60);

  const parts: string[] = [];
  if (hours > 0) parts.push(`${hours}h`);
  if (minutes > 0) parts.push(`${minutes}m`);
  if (secs > 0 && hours === 0) parts.push(`${secs}s`); // Only show seconds if less than an hour

  return parts.length > 0 ? parts.join(' ') : '0s';
}

/**
 * Formats a date to a readable format (e.g., "Jan 15, 2025")
 */
export function formatDate(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid Date';

  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  };

  return date.toLocaleDateString('en-US', options);
}

/**
 * Formats a time to readable format (e.g., "3:30 PM")
 */
export function formatTime(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid Time';

  const options: Intl.DateTimeFormatOptions = {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  };

  return date.toLocaleTimeString('en-US', options);
}

/**
 * Formats a datetime to readable format (e.g., "Jan 15, 2025 3:30 PM")
 */
export function formatDateTime(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid DateTime';

  return `${formatDate(dateString)} ${formatTime(dateString)}`;
}

/**
 * Formats time for EPG display (e.g., "15:30")
 */
export function formatEPGTime(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return '--:--';

  const hours = padZero(date.getHours());
  const minutes = padZero(date.getMinutes());

  return `${hours}:${minutes}`;
}

/**
 * Formats relative time (e.g., "2 hours ago", "in 30 minutes")
 */
export function formatRelativeTime(dateString: string): string {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid Date';

  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffSecs = Math.abs(Math.floor(diffMs / 1000));
  const isPast = diffMs > 0;

  if (diffSecs < 60) {
    return 'just now';
  }

  const diffMins = Math.floor(diffSecs / 60);
  if (diffMins < 60) {
    return isPast ? `${diffMins} minute${diffMins > 1 ? 's' : ''} ago` : `in ${diffMins} minute${diffMins > 1 ? 's' : ''}`;
  }

  const diffHours = Math.floor(diffMins / 60);
  if (diffHours < 24) {
    return isPast ? `${diffHours} hour${diffHours > 1 ? 's' : ''} ago` : `in ${diffHours} hour${diffHours > 1 ? 's' : ''}`;
  }

  const diffDays = Math.floor(diffHours / 24);
  if (diffDays < 30) {
    return isPast ? `${diffDays} day${diffDays > 1 ? 's' : ''} ago` : `in ${diffDays} day${diffDays > 1 ? 's' : ''}`;
  }

  const diffMonths = Math.floor(diffDays / 30);
  if (diffMonths < 12) {
    return isPast ? `${diffMonths} month${diffMonths > 1 ? 's' : ''} ago` : `in ${diffMonths} month${diffMonths > 1 ? 's' : ''}`;
  }

  const diffYears = Math.floor(diffMonths / 12);
  return isPast ? `${diffYears} year${diffYears > 1 ? 's' : ''} ago` : `in ${diffYears} year${diffYears > 1 ? 's' : ''}`;
}

/**
 * Formats file size in bytes to human-readable format (e.g., "1.5 MB")
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes';

  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
}

/**
 * Formats bitrate in kbps to human-readable format (e.g., "5.2 Mbps")
 */
export function formatBitrate(kbps: number): string {
  if (kbps < 1000) {
    return `${kbps.toFixed(0)} kbps`;
  }

  const mbps = kbps / 1000;
  return `${mbps.toFixed(1)} Mbps`;
}

/**
 * Formats a percentage with specified decimal places
 */
export function formatPercentage(value: number, decimals: number = 0): string {
  return `${value.toFixed(decimals)}%`;
}

/**
 * Truncates text to a specified length with ellipsis
 */
export function truncateText(text: string, maxLength: number): string {
  if (text.length <= maxLength) return text;
  return `${text.substring(0, maxLength - 3)}...`;
}

/**
 * Capitalizes the first letter of a string
 */
export function capitalizeFirst(text: string): string {
  if (!text) return '';
  return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
}

/**
 * Converts a string to title case (e.g., "hello world" -> "Hello World")
 */
export function toTitleCase(text: string): string {
  if (!text) return '';
  return text
    .toLowerCase()
    .split(' ')
    .map(word => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');
}

/**
 * Pads a number with leading zeros
 */
function padZero(num: number, length: number = 2): string {
  return num.toString().padStart(length, '0');
}

/**
 * Formats a number with thousand separators (e.g., 1000000 -> "1,000,000")
 */
export function formatNumber(num: number): string {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

/**
 * Formats viewer count (e.g., 1500 -> "1.5K", 1000000 -> "1M")
 */
export function formatViewerCount(count: number): string {
  if (count < 1000) return count.toString();
  if (count < 1000000) return `${(count / 1000).toFixed(1)}K`;
  return `${(count / 1000000).toFixed(1)}M`;
}
