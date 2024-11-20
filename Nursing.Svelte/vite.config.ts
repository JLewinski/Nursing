import { sveltekit } from "@sveltejs/kit/vite"
import { defineConfig } from "vite"

export default defineConfig({
  plugins: [sveltekit()],
  server: {
    fs: {
      strict: false,
    }
  },
  css:{
    preprocessorOptions: {
      scss: {
        quietDeps: true,
      }
    }
  }
  // TODO: Add PWA plugin configuration
})
