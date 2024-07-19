import { cookies, headers } from 'next/headers';

import { NextApiRequest } from 'next';
import { authOptions } from '../api/auth/[...nextauth]/route';
import { getServerSession } from 'next-auth';
import { getToken } from 'next-auth/jwt';

export async function getSession() {
  return await getServerSession(authOptions);
}

export async function getCurrentUser() {
  try {
    const session = await getSession();
    return session?.user;
  } catch (error) {
    return null;
  }
}

export async function getTokenWorkaround() {
  const req = {
    headers: Object.fromEntries(headers() as Headers),
    cookies: Object.fromEntries(
      cookies()
        .getAll()
        .map((c) => [c.name, c.value])
    ),
  } as NextApiRequest;

  return await getToken({ req });
}
