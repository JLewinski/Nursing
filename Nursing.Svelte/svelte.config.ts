import adapter from "@sveltejs/adapter-static";
import { vitePreprocess } from "@sveltejs/vite-plugin-svelte";

const config: import('@sveltejs/kit').Config = {
  preprocess: vitePreprocess(),
  kit: {
    adapter: adapter({
      fallback: 'index.html'
    }),
    files: {
      serviceWorker: 'static/service-worker'
    }
  }
};

export default config;
