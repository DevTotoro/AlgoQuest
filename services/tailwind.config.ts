import type { Config } from 'tailwindcss';

const config: Config = {
  content: ['./src/**/*.{ts,tsx}'],

  theme: {
    extend: {},
  },

  plugins: [require('daisyui')],

  daisyui: {
    themes: ['corporate'],
  },
};

export default config;
