// store.js
import { writable } from 'svelte/store';

// List of all cluster bookings
export const clusterListStore = writable([]);
// List of all virtual machine bookings
export const vmListStore = writable([]);
// Temp store information about a specific booking. Used to send the booking a user clicked on to a modal etc
export const selectedBookingStore = writable(null);

// List of booking types a user wants to see on their homepage. Shows cluster and vm bookings by default
export const selectedBookingTypes = writable([]);
// List of booking statuses a user wants to see. Shows Confirmed and other by default
export const selectedBookingStatus = writable(null);
// Type of booking preview to show, can be 'Card' or 'Table' view
export const selectedBookingPreview = writable('card');

// Basic information about the user, like id, name, email, role etc
export const userStore = writable(null);
