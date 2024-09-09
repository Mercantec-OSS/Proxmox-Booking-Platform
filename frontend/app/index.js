import { handler } from './build/handler.js';
import 'dotenv/config';
import http from 'http';

// Default to 3000 if PORT is not set in env
const port = process.env.PORT || 3000;

http.createServer(handler).listen(port, () => {
	console.log(`Server running on port ${port}`);
});
