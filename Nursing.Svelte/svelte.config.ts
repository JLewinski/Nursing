import adapter from "svelte-adapter-deno";
import { vitePreprocess } from "@sveltejs/vite-plugin-svelte";

/** @type {import('@sveltejs/kit').Config} */
const config = {
  preprocess: vitePreprocess(),
  kit: {
    adapter: adapter(),
  },
  compilerOptions: {
    runes: true,
  },
};

export default config;
