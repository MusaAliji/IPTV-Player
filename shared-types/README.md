# IPTV Player - Shared Types

Shared TypeScript types, constants, and utilities for the IPTV Player application. This package provides a single source of truth for data structures and common functionality across web, mobile, and TV client applications.

## Features

- **Type Definitions**: Comprehensive TypeScript types for all entities
- **Constants**: API endpoints, app configuration, and common values
- **Utilities**: Validation, formatting, and HTTP client helpers
- **Type Safety**: Ensures consistency across all client applications

## Installation

```bash
npm install @iptv-player/shared-types
```

## Usage

### Types

```typescript
import {
  Content,
  Channel,
  User,
  EPGProgram,
  ContentType,
  UserRole,
  StreamQuality
} from '@iptv-player/shared-types';

const content: Content = {
  id: 1,
  title: 'Sample Movie',
  streamUrl: 'https://example.com/stream',
  type: ContentType.Movie,
  // ...
};
```

### Constants

```typescript
import {
  API_ENDPOINTS,
  PAGINATION,
  STREAMING_CONFIG
} from '@iptv-player/shared-types';

// Use API endpoints
const contentUrl = `${API_BASE_PATH}${API_ENDPOINTS.CONTENT.BY_ID(123)}`;

// Use pagination defaults
const limit = PAGINATION.DEFAULT_LIMIT;
```

### Utilities

```typescript
import {
  validateEmail,
  formatDuration,
  formatDate,
  makeRequest,
  buildUrl
} from '@iptv-player/shared-types';

// Validate email
const result = validateEmail('user@example.com');
if (!result.isValid) {
  console.log(result.errors);
}

// Format duration
const formatted = formatDuration(3665); // "01:01:05"

// Format date
const date = formatDate('2025-01-15T12:00:00Z'); // "Jan 15, 2025"

// Make HTTP request
const response = await makeRequest('/api/content', {
  method: 'GET',
  headers: { 'Authorization': 'Bearer token' }
});
```

## Package Structure

```
shared-types/
├── src/
│   ├── types/
│   │   ├── content.types.ts    # Content and media types
│   │   ├── epg.types.ts        # EPG types
│   │   ├── user.types.ts       # User and auth types
│   │   ├── streaming.types.ts  # Streaming and playback types
│   │   └── index.ts
│   ├── constants/
│   │   ├── api.constants.ts    # API endpoints
│   │   ├── app.constants.ts    # App configuration
│   │   └── index.ts
│   ├── utils/
│   │   ├── validators.ts       # Validation functions
│   │   ├── formatters.ts       # Formatting functions
│   │   ├── http-client.ts      # HTTP helpers
│   │   └── index.ts
│   └── index.ts
├── tests/                      # Unit tests
├── package.json
├── tsconfig.json
└── README.md
```

## Type Categories

### Content Types
- `Content` - Media content entity
- `Channel` - Live TV channel entity
- `ContentType` - Content type enum
- `CreateContentDto`, `UpdateContentDto` - DTOs for content operations
- `ContentFilters`, `PaginatedContentResponse` - Search and pagination

### EPG Types
- `EPGProgram` - Electronic Program Guide entry
- `EPGSchedule` - Channel schedule
- `EPGGrid` - Multi-channel grid data
- `ChannelSchedule` - Current and upcoming programs

### User Types
- `User` - User entity
- `UserRole` - User role enum
- `UserPreference` - User preferences
- `ViewingHistory` - Viewing history entry
- `AuthResponse` - Authentication response
- DTOs for registration, login, password reset, etc.

### Streaming Types
- `StreamQuality` - Quality level enum
- `PlaybackState` - Playback state enum
- `StreamInfo` - Stream information
- `PlayerConfig` - Player configuration
- `PlayerState` - Player state
- `SubtitleTrack`, `AudioTrack` - Track information

## Validation

The package provides comprehensive validation functions:

- `validateEmail(email)` - Email validation
- `validateUsername(username)` - Username validation
- `validatePassword(password)` - Password validation
- `validateUrl(url)` - URL validation
- `validateStreamUrl(url)` - Stream URL validation
- `validateContentTitle(title)` - Content title validation
- `validateRating(rating)` - Rating validation
- `validateDateString(date)` - Date validation
- `validateTimeRange(start, end)` - Time range validation

All validators return a `ValidationResult` object:

```typescript
interface ValidationResult {
  isValid: boolean;
  errors: string[];
}
```

## Formatting

Formatting utilities for common display needs:

- `formatDuration(seconds)` - Format duration (HH:MM:SS)
- `formatDurationHuman(seconds)` - Human-readable duration (1h 30m)
- `formatDate(dateString)` - Format date (Jan 15, 2025)
- `formatTime(dateString)` - Format time (3:30 PM)
- `formatDateTime(dateString)` - Format date and time
- `formatEPGTime(dateString)` - EPG time format (15:30)
- `formatRelativeTime(dateString)` - Relative time (2 hours ago)
- `formatFileSize(bytes)` - File size (1.5 MB)
- `formatBitrate(kbps)` - Bitrate (5.2 Mbps)
- `formatPercentage(value)` - Percentage (85.5%)
- `truncateText(text, maxLength)` - Truncate with ellipsis
- `formatNumber(num)` - Number with separators (1,000,000)
- `formatViewerCount(count)` - Viewer count (1.5K, 2M)

## HTTP Client

HTTP client helpers for API communication:

- `makeRequest(url, options)` - Make HTTP request with timeout
- `get(url, params, headers)` - GET request
- `post(url, body, headers)` - POST request
- `put(url, body, headers)` - PUT request
- `patch(url, body, headers)` - PATCH request
- `del(url, headers)` - DELETE request
- `buildUrl(baseUrl, params)` - Build URL with query params
- `createDefaultHeaders(includeAuth, token)` - Create default headers
- `retryRequest(requestFn, maxRetries, delayMs)` - Retry failed requests

## Development

```bash
# Install dependencies
npm install

# Build the package
npm run build

# Run tests
npm test

# Run tests with coverage
npm run test:coverage

# Watch mode
npm run test:watch
```

## Testing

All utilities are thoroughly tested. Run the test suite with:

```bash
npm test
```

Coverage threshold is set to 80% for branches, functions, lines, and statements.

## License

MIT
