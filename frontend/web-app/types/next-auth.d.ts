import { DefaultSession } from 'next-auth';

declare module 'next-auth' {
  interface Session extends DefaultSession['user'] {
    user: {
      username: string;
    };
  }

  interface Profile {
    username: string;
  }

  interface User {
    username: string;
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    username: string;
  }
}
