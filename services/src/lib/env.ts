export const env = {
  NODE_ENV: process.env.NODE_ENV,

  API_URL: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:3000',
  API_KEY: process.env.API_KEY ?? 'default',
};
