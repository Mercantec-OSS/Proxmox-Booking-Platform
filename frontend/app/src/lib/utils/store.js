// store.js
import { writable } from 'svelte/store';

// List of all virtual machine bookings
export const vmListStore = writable([]);
// Temp store information about a specific booking. Used to send the booking a user clicked on to a modal etc
export const selectedBookingStore = writable(null);

// Basic information about the user, like id, name, email, role etc
export const userStore = writable(null);
