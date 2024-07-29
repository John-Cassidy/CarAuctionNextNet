'use client'

import { Auction, Bid } from '@/types';
import React, { useEffect, useState } from 'react';

import BidItem from './BidItem';
import EmptyFilter from '@/app/components/EmptyFilter';
import Heading from '@/app/components/Heading';
import { User } from 'next-auth';
import { getBidsForAuction } from '@/app/actions/auctionActions';
import toast from 'react-hot-toast';
import { useBidStore } from '@/hooks/useBidStore';

type Props = {
  user: User | null;
  auction: Auction;
};

export default function BidList({ user, auction }: Props) {
  const [loading, setLoading] = useState(true);
  const bids = useBidStore((state) => state.bids);
  const setBids = useBidStore((state) => state.setBids);

  useEffect(() => {
    getBidsForAuction(auction.id)
      .then((res: any) => {
        if (res.error) {
          throw res.error;
        }
        setBids(res as Bid[]);
      })
      .catch((err) => {
        toast.error(err.message);
      })
      .finally(() => {
        setLoading(false);
      });
  }, [auction.id, setLoading, setBids]);

  if (loading) {
    return <span>Loading bids...</span>;
  }

  return (
    <div className='rounded-lg shadow-md'>
      <div className='py-2 px-4 bg-white'>
        <div className='sticky top-0 bg-white p-2'>
          <Heading title='Bids' />
        </div>
      </div>

      <div className='overflow-auto h-[400px] flex flex-col-reverse px-2'>
        {bids.length === 0 ? (
          <EmptyFilter
            title='No bids for this item'
            subtitle='Please feel free to make a bid'
          />
        ) : (
          <>
            {bids.map((bid) => (
              <BidItem key={bid.id} bid={bid} />
            ))}
          </>
        )}
      </div>
    </div>
  );
}
