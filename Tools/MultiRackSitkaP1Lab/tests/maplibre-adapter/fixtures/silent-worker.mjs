// Test stub for B17 (worker failure): a same-origin module worker that loads cleanly but never
// answers MapLibre, so the map can never finish loading. The adapter's load watchdog must then
// fall back to Clean Woodland with the "Map display unavailable" copy.
self.addEventListener('message', () => {});

// B24 (repair cycle 3): fetch this worker's own script URL once. The worker's CDP session then
// reports an ordinary Fetch response with the same URL as its startup Script response, and the
// audit must count only the Script response (CDP resource type "Script") as startup evidence.
fetch(self.location.href, { cache: 'no-store' }).then((r) => r.arrayBuffer()).catch(() => {});
