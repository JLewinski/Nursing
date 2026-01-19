# Nursing App - AI Coding Guide

## Project Overview
**Nursing with the Saints** is a Svelte 5-based PWA (Progressive Web App) for tracking two concurrent timers in nursing sessions. The app emphasizes offline-first functionality, accurate timer persistence, and seamless data sync to a backend API.

## Architecture Essentials

### Tech Stack
- **Framework**: SvelteKit 2.0 with Svelte 5 (latest reactive features)
- **Storage**: IndexedDB (`NursingDB`) for sessions and settings
- **PWA**: Service Workers for offline support, Web App Manifest for installation
- **Build**: Vite + SvelteKit with Deno adapter
- **Runtime**: Deno (instead of Node.js)
- **UI**: Bootstrap 5 + Bootstrap Icons, Chart.js for analytics

### Core Data Flow
```
Timer Events (ISO timestamps) → TimerStore (in-memory state + localStorage)
                              → Session Duration Calculations
                              → IndexedDB (persistent sessions)
                              → API Sync (when online)
```

### Key Stores (Svelte.ts Pattern)
All stores use **Svelte 5 reactive classes** with explicit state/persistence:

- **`timerStore.svelte.ts`**: `TimerState` class manages `events[]` (start/stop pairs) and `activeTimer`
  - Uses ISO timestamps; duration calculated on-demand from event history
  - `toggle()` adds events atomically; `calculateDuration()` sums completed intervals
  - Persists to localStorage key `"timerStore"`

- **`settingsStore.svelte.ts`**: `SettingsState` stores `estimatedInterval`, notification prefs
  - Persists to localStorage key `"nursing-settings"`

- **`lastSessionStore.svelte.ts`**: Tracks `startTime`, previous session metadata
  - Used for UI: "last timer used" indicator, "time until next session"

### Database (IndexedDB)
**Database**: `NursingDB` (version 1)

**Object Store: `sessions`**
```typescript
{
  id: string (keyPath),
  startTime: string (ISO),
  endTime: string (ISO) | undefined,
  lastSide: 'left' | 'right',
  leftDuration: number (milliseconds),
  rightDuration: number (milliseconds),
  lastUpdated: string (ISO),
  created: string (ISO),
  deleted?: string (soft-delete timestamp)
}
```
**Indices**: `startTime`, `lastUpdated`, `deleted` (for soft-deletes)

**Object Store: `syncState`** (for backend sync metadata)
```typescript
{
  id: string,
  lastSyncTimestamp: string | null,
  syncStatus: 'idle' | 'syncing' | 'error'
}
```

## Critical Patterns

### Timer Accuracy Strategy
Timers **store precise event timestamps**, not durations:
- Each toggle creates a `TimerEvent {type: 'start'|'stop', timer: 'left'|'right', timestamp: ISO}`
- Duration is **calculated on-demand** by summing stop-start intervals
- This survives app suspend, device sleep, and clock adjustments
- **Never store pre-calculated durations in events**; duration is derived

### Data Persistence Flow
1. **In-Session**: TimerStore in-memory state + localStorage (fast)
2. **Session End**: Save to IndexedDB via `Database.saveSession()`
3. **Sync**: `SyncManager` pushes to API (soft-delete support via `deleted` field)
4. **Conflict Resolution**: Timestamp-based (last-update-wins logic)

### Soft Deletes
Deleted sessions have a `deleted: ISO_timestamp` field instead of full removal:
- Allows sync to detect deletions
- Query filters typically use `!session.deleted`
- See `Database.deleteSession()` vs. `removeSession()`

### Service Worker Caching Strategy
- **Install**: Cache all static assets and app files
- **Fetch**: Assets from CACHE (stale), API calls try network-first, fallback to cache
- Only handles `GET` requests (POST/PUT ignored)
- Cleanup old caches on activation

## Development Workflows

### Build Commands
```bash
deno task dev          # Local dev server (Vite)
deno task build        # Production build → build/
deno task preview      # Test production locally
deno task prod         # Run Deno server
deno task docker_build # Build Docker image
deno task docker_run   # Run in Docker (port 6969)
```

### Database Initialization
`Database` class auto-initializes via `init()`:
- Opens IndexedDB with version migration system
- Creates object stores and indices on first run
- Loads old data via `migrateOldData()` if present

### Adding DB Migrations
1. Add new migration object to `migrations[]` in [migrations.ts](src/lib/db/migrations.ts)
2. Increment version number; `applyMigrations()` runs all > current version
3. Old data migration logic in [migration_old.ts](src/lib/db/migration_old.ts)

## File Organization Conventions

| Path | Purpose |
|------|---------|
| `src/routes/` | SvelteKit pages (`+page.svelte`, layout, API routes) |
| `src/lib/stores/*.svelte.ts` | Reactive store classes (Svelte 5 pattern) |
| `src/lib/db/` | IndexedDB schema, migrations, data layer |
| `src/lib/types/` | TypeScript interfaces (Session, TimerEvent, SyncState, etc.) |
| `src/lib/utils/` | Sync, time calculations, notifications, service worker setup |
| `src/lib/components/` | Reusable Svelte components (Timer, Duration, ConfirmDialog, etc.) |
| `src/lib/config/` | Constants (reminder intervals, notification settings) |
| `static/` | Web App Manifest, PWA assets |

## Common Tasks

### Adding a New Store Property
1. Update TypeScript interface in `src/lib/types/index.ts`
2. Add to store class in `src/lib/stores/` as `$state()` property
3. Add save/load logic in `constructor()` and persistence method
4. Export singleton instance at module bottom

### Modifying Timer Logic
- **Always preserve**: Event timestamp accuracy via ISO strings
- **Test**: `TimerState.calculateDuration()` with edge cases (active timers, gaps, resets)
- **Never**: Store duration directly in event; calculate on-demand

### PWA/Service Worker Changes
- Edit [service-worker.ts](src/service-worker.ts)
- Run `deno task build` to regenerate `$service-worker` manifest
- Test offline behavior: DevTools → Network → Offline, then reload

### Database Changes
- Schema updates → new migration in [migrations.ts](src/lib/db/migrations.ts)
- Always increment `DB_VERSION` in `Database` class
- Use soft-deletes for user data (preserve sync history)

## Integration Points

### Backend API (Not Yet Implemented)
- `SyncManager` stub in [sync.ts](src/lib/utils/sync.ts)
- Expected flow: localStorage → IndexedDB → API (with timestamp-based conflict resolution)
- Sessions include `lastUpdated` for version detection

### Push Notifications
- Permission handling in [notifications.ts](src/lib/utils/notifications.ts)
- Configured via `settingsStore.notifications`
- Service worker support for background notifications (planned)

## Testing Offline Behavior
1. DevTools → Network tab → Offline checkbox
2. Verify timers still work (localStorage + in-memory state)
3. Verify session save on next online (IndexedDB backup)
4. Check service worker cache in DevTools → Application → Cache Storage

## Pages Overview

| Route | Purpose | Key Components |
|-------|---------|-----------------|
| `/` | Active timer session | Timer (left/right), finish/reset buttons, session metadata |
| `/history` | Session management & analytics | GridJS table, filters, delete/edit, weekly/daily stats |
| `/settings` | User preferences | Theme selector, data delete, interval config, auto-sync toggle |
| `/account` | Auth & sync control | Sign in/out, sync status, manual sync trigger |

## Notes for AI Agents

- **Svelte 5 runes**: Use `$state()` for reactive properties, `$derived()` for computed values
- **ISO timestamps everywhere**: All dates stored as ISO strings; parse on load if needed
- **Avoid localStorage for sessions**: Use IndexedDB (with localStorage as emergency fallback for stores)
- **Test PWA offline**: Service Worker caching is critical; verify with DevTools offline mode
- **Soft-deletes first**: When implementing deletion, add `deleted` timestamp instead of hard-delete
