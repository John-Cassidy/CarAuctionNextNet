export { default } from 'next-auth/middleware';

// Paths to protect and pages to redirect to if not signed in
export const config = {
  matcher: ['/session'],
  pages: {
    signIn: '/api/auth/signin',
  },
};
