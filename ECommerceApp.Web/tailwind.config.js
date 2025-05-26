/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,html,js}",
    "./wwwroot/**/*.{cshtml,html,js}"
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [],
}