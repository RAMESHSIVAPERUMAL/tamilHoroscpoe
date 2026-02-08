# Documentation

This folder contains guides for key features of the Tamil Horoscope Desktop application.

## Getting Started

**New to the application?** Start here:

1. Read the main [project README](../README.md) for overview
2. Read this file for documentation structure
3. Choose your documentation path below

## Documentation Structure

### For End Users

**Goal:** Understand how to use the application

Start with: [QuickReference.md](QuickReference.md)
- How to search for birth places
- How to calculate horoscope
- Auto-fill features
- Quick troubleshooting

### For Developers

**Goal:** Understand how to extend or modify the application

Start with: [QuickReference.md](QuickReference.md) (Developer section)
- How to use BirthPlaceService in code
- API configuration options
- Adding custom places
- Code locations

Then read: [OnlineOfflineBirthPlacePicker.md](OnlineOfflineBirthPlacePicker.md)
- Complete architecture
- Implementation details
- API integration
- Security considerations

## Available Documentation

| Document | Purpose | Audience |
|----------|---------|----------|
| [QuickReference.md](QuickReference.md) | Quick start guide with common tasks | Everyone |
| [OnlineOfflineBirthPlacePicker.md](OnlineOfflineBirthPlacePicker.md) | Birth place picker architecture and implementation | Developers |

## Key Features

The application includes:
- Online/offline birth place search
- Real-time auto-complete
- Automatic coordinate and timezone population
- Graceful error handling
- Offline-first strategy (checks local cache before API)

## Common Questions

### How do I search for a birth place?
See QuickReference.md - User Guide section

### How do I add custom places?
See QuickReference.md - Adding Places Manually section

### How do I switch to a different geocoding API?
See QuickReference.md - API Configuration section or OnlineOfflineBirthPlacePicker.md

### How does offline mode work?
See OnlineOfflineBirthPlacePicker.md - How It Works section

### What APIs are supported?
- OpenStreetMap Nominatim (current default)
- Google Maps (configurable)
- Mapbox (configurable)
- Azure Maps (configurable)

See QuickReference.md - API Configuration section

## Build Status

- Build: Successful
- Tests: 86/86 passing
- Status: Production Ready

---

For issues or questions, see the troubleshooting section in QuickReference.md or visit the GitHub repository.

