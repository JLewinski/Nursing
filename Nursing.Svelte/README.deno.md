# Deno Migration Guide

This project has been migrated from npm/Node.js to Deno.

## Setup

### Install Deno
```bash
# macOS/Linux
curl -fsSL https://deno.land/install.sh | sh

# macOS (Homebrew)
brew install deno
```

### Install Dependencies
```bash
deno install
```

This will read `deno.json` and cache all npm dependencies automatically.

## Development Workflow

### Run Development Server
```bash
deno task dev
```

### Build for Production
```bash
deno task build
```

### Preview Production Build
```bash
deno task preview
```

### Run Production Server
```bash
deno task prod
```

### Docker Commands
```bash
deno task docker_build  # Build Docker image
deno task docker_run    # Run container on port 6969
```

## Key Changes from npm

1. **Configuration**: `deno.json` replaces `package.json` for tasks and imports
2. **Adapter**: Using `svelte-adapter-deno` instead of `@sveltejs/adapter-node`
3. **Import Maps**: npm packages accessed via `npm:` specifier in `deno.json`
4. **Node Modules**: Auto-managed by Deno (no manual `npm install`)
5. **Lock File**: Deno manages dependencies with `deno.lock` (auto-generated)

## Import Style

Deno uses npm compatibility mode. All npm packages are prefixed with `npm:` in the import map:
```typescript
import { sveltekit } from "@sveltejs/kit/vite";  // Works via import map
```

The `imports` section in `deno.json` maps these to `npm:` specifiers.

## Permissions

Deno requires explicit permissions. Common flags:
- `-A` or `--allow-all`: All permissions (used in dev)
- `--allow-net`: Network access
- `--allow-read`: File system read
- `--allow-write`: File system write
- `--allow-env`: Environment variables

Production server uses minimal permissions:
```bash
deno run --allow-net --allow-read --allow-env build/index.js
```

## Node Modules Directory

`deno.json` has `"nodeModulesDir": "auto"` which:
- Creates `node_modules/` for compatibility with tools expecting it
- Allows Vite and SvelteKit to work without modification
- Auto-managed by Deno (no need to gitignore)

## Troubleshooting

### "Module not found" errors
Run `deno install` to ensure all dependencies are cached.

### Type errors in imports
Deno uses built-in TypeScript. If you see import errors, check that:
1. The import exists in `deno.json` imports map
2. The npm package version is correct

### Service Worker issues
Service worker runs in the browser (not Deno runtime), so no changes needed.

## Migration Checklist

- [x] Create `deno.json` with tasks and import map
- [x] Update `svelte.config.js` to use `svelte-adapter-deno`
- [x] Update documentation to reference `deno task` commands
- [x] Verify Dockerfile uses Deno runtime
- [ ] Test all features in development mode
- [ ] Test production build and server
- [ ] Test Docker container deployment
- [ ] Remove `package.json` (optional, can keep for reference)
