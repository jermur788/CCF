// Test stub for B17 (worker failure): a same-origin module worker that loads cleanly but never
// answers MapLibre, so the map can never finish loading. The adapter's load watchdog must then
// fall back to Clean Woodland with the "Map display unavailable" copy.
self.addEventListener('message', () => {});
