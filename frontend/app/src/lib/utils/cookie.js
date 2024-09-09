export function getCookie(name) {
  return (document.cookie.split('; ').find((row) => row.startsWith(name + '=')) || '').split('=')[1];
}

export function deleteCookie(name) {
  document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}
