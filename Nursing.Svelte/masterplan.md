# Nursing Application Master Plan

## Overview
A Progressive Web Application (PWA) built with SvelteKit for tracking nursing sessions, focusing on reliable timer functionality and offline-first operation.

## Core Technical Architecture

### Frontend Stack
- **Framework**: SvelteKit
- **PWA Features**: 
  - Service Workers for offline functionality
  - Web App Manifest for installation
  - Background sync for data persistence
  - Push API for notifications

### Data Layer Architecture
1. **Local Storage Layer (IndexedDB)**
   - Database Name: "NursingDB"
   - Stores:
     - sessions: {id, timerEvents: [{type: 'start|stop', timer: 'left|right', timestamp}], startTime, endTime, lastUpdated, created, deleted}
     - settings: {theme, estimatedInterval, notificationPreferences}
     - syncState: {lastSyncTimestamp, syncStatus}

2. **Timer Implementation Strategy**
   - Store precise timestamps for start/stop events
   - Calculate current duration based on event history
   - Maintain accuracy regardless of device sleep/background state
   - Regular UI updates using requestAnimationFrame

3. **Notification System**
   - Implement using Push API and service workers
   - Support background notifications
   - Global configuration for notification timing
   - Permission handling and user preferences

### Core Features Implementation

1. **Timer System**
   - Event-based timer tracking using timestamps
   - Accurate duration calculation
   - Resilient to app suspension/background
   - Simple atomic operations for timer switches

2. **PWA Implementation**
   - Full offline capability
   - Add to home screen functionality
   - Background processing
   - Push notifications

3. **Sync Functionality (Simplified)**
   - Basic sync with backend for backup
   - Partner data sharing support
   - Conflict resolution based on timestamps

## Development Phases

### Phase 1: Core PWA Setup
- SvelteKit project setup with PWA capabilities
- Service worker implementation
- Manifest configuration
- Installation flow

### Phase 2: Timer Implementation
- Timestamp-based event tracking
- Duration calculations
- UI updates
- State persistence

### Phase 3: Notification System
- Push notification setup
- Background notification support
- Permission handling
- Global configuration

### Phase 4: Data Management
- Basic sync functionality
- Partner sharing features
- Backup/restore capabilities

## Technical Considerations

### Timer Accuracy
- Store precise ISO timestamps for all events
- Calculate duration on demand
- Handle time zone changes gracefully
- Account for device clock adjustments

### PWA Requirements
- Reliable offline operation
- Background processing capability
- Push notification support
- Installation experience

## Initial Focus Areas
1. PWA setup and installation flow
2. Robust timer implementation
3. Notification system
4. Basic data persistence