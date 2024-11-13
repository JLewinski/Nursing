import { sveltekit } from "@sveltejs/kit/vite"
import { defineConfig } from "vite"

export default defineConfig({
  plugins: [sveltekit()],
  server: {
    fs: {
      strict: false,
    },
    proxy: {
      '/api': 'http://localhost:5173',
    }
  },
  // TODO: Add PWA plugin configuration
})
