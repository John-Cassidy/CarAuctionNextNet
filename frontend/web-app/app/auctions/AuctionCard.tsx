import { Auction } from '@/types';
import CarImage from './CardImage';
import CountdownTimer from './CountdownTimer';
import CurrentBid from './CurrentBid';
import Image from 'next/image';
import Link from 'next/link';
import React from 'react';

type Props = {
  auction: Auction;
};

export default function AuctionCard({ auction }: Props) {
  return (
    <Link href={`/auctions/details/${auction.id}`} className='group'>
      <div className='w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden'>
        <div>
          <CarImage imageUrl={auction.imageUrl} />
          <div className='absolute bottom-2 left-2'>
            <CountdownTimer auctionEnd={auction.auctionEnd} />
          </div>
          <div className='absolute top-2 right-2'>
            <CurrentBid
              amount={auction.currentHighBid}
              reservePrice={auction.reservePrice}
            />
          </div>
        </div>
      </div>
      <div className='flex justify-between items-center mt-4'>
        <h3 className='text-gray-700'>
          {auction.make} {auction.model}
        </h3>
        <p className='font-semibold text-sm'>{auction.year}</p>
      </div>
    </Link>
  );
}
