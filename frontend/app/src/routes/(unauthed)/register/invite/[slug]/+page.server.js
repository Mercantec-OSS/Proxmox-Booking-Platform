import { error } from '@sveltejs/kit';

export async function load({ params }) {
  const inviteKey = params.slug;

  // Validate invite key format (UUID v4)
  const uuidV4Regex = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  if (!uuidV4Regex.test(inviteKey)) {
    throw error(400, 'Invalid invite key format');
  }

  return {
    inviteKey
  };
}
