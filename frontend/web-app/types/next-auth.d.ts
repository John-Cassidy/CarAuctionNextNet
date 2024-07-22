import { DefaultSession } from 'next-auth';

declare module 'next-auth' {
  interface Session extends DefaultSession['user'] {
    user: {
      id: string;
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
    access_token?: string;
  }
}
