'use client';

import React, { useState } from 'react';

import { Button } from 'flowbite-react';
import { deleteAuction } from '@/app/actions/auctionActions';
import { toast } from 'react-hot-toast';
import { useRouter } from 'next/navigation';

type Props = {
  id: string;
};

export default function DeleteButton({ id }: Props) {
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  function doDelete() {
    setLoading(true);
    deleteAuction(id)
      .then((res) => {
        if (res.error) throw res.error;
        router.push('/');
      })
      .catch((error) => {
        toast.error(error.status + ' ' + error.message);
      })
      .finally(() => setLoading(false));
  }

  return (
    <Button color='failure' isProcessing={loading} onClick={doDelete}>
      Delete Auction
    </Button>
  );
}
