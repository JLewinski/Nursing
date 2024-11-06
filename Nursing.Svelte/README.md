# Nursing with the Saints

This application is build using Sveltekit and deno.
It is a front end application for time tracking. It will be mostly used on mobile devices.
It has a navigation bar at the bottom with links to different pages.

The main functionality of this application is to track two timers in a given session.

## Sessions

A session consists of the following data.

- Left Timer Duration
- Right Timer Duration
- Last Timer Used
- Session Start Time
- Session End Time
- Id
- Last Updated
- Created
- Deleted

These sessions should be stored locally in IndexedDB and synced to the api. The user should be able to modify and delete previous sessions from the history page.

## Pages

This application has several pages.

1. Home
2. History
3. Settings
4. Account

The user is able to navigate to these pages via a naviagation bar at the bottom of their screen. The navigation bar should show only icons on small screens and add text next to the icons if it is a larger screen. The navigation item for the account should also indicate the sync status.

### Home Page

This should be a clean page focused on the timers. Only the necessary information for the session should be shown with an exeption for optional motivational quotes and prayers.

#### States

The home page has 3 states.

##### New User

This state occures when there are no prior sessions. The page should only display the two timers. This state ends when a timer is toggled.

##### Session in Progress

This state occures when either timer has been started and the session has not been finished. When in this state the page should display the following:

- the two timers
- a finish button
- a reset button
- an indication of wich timer was used last in the last session (if there was a last session)
- The total time of the two timers combined

##### Session Finished

This state occures when a session has finished. When in this state the page should display the following

- two timers with default values
- an indication of which timer was used last in the last session
- when the last session ended
- how long it has been since the last session ended
- the estimated time of the next session based on the previous session's start time
- how long until the estimated time of the next session

### History Page

- Show grid of past sessions
- Filter grid by given date range
- Modify selected session
- Delete selected session
- Statistics
  - Weekly
  - Daily
  - Graphs
  - Charts

### Settings Page

This page should allow the user to

- choose a theme
  - auto (default)
  - dark
  - light
- delete their local data
- set the estimated time between each session start time
- toggle auto-sync (default off)

### Account Page

All users should be able to

- Display Sync Status including time last synced
- Sign in
- Manual Sync
- Sign out
- Delete account
- Invite another user to sync sessions
- Accept invite from other user
- Change Password

Administrators should also be able to

- Register new users
- Delete users

## Sync and Data

This is for offline-first. Data should be stored in IndexedDB. Conflicts should take values from the one that was most recently updated. 

- There is already an API backend built in .NET Core ../Nursing.API/Nursing.API.csproj
- When there are conflicts the session with the most recent update timestamp gets priorety

## Notifications and Alerts

The notifications will be implemented as native notifications with background support. Global configuration settings will determine when notifications are triggered (e.g., timer duration thresholds). The primary use case is reminding users to switch sides after a configured duration.

## Components

### Timers

These timers should be big, colored circles with the duration inside in digital format. They should be easy to press when using a mobile device. They should be labeled "Left" and "Right" with the text above each timer. Colors and animations should be used to subtly indicate which timer is active. The timers cannot be on at the same time. When switching from one timer to another it should only take one tap with no confirmation. There should be a confirmation when resetting.
