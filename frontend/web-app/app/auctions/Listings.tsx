'use client';

import { Auction, PagedResult } from '@/types';
import React, { useEffect, useState } from 'react';

import AppPagination from '../components/AppPagination';
import AuctionCard from './AuctionCard';
import { getData } from '../actions/auctionActions';

export default function Listings() {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [pageCount, setPageCount] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() => {
    getData(pageNumber).then((data: PagedResult<Auction>) => {
      setAuctions(data.results);
      setPageCount(data.pageCount);
    });
  }, [pageNumber]);

  return (
    <>
      <div className='grid grid-cols-4 gap-6'>
        {auctions.map((auction: Auction) => (
          <AuctionCard key={auction.id} auction={auction} />
        ))}
      </div>
      <div className='flex justify-center mt-4'>
        <AppPagination
          pageChanged={setPageNumber}
          currentPage={pageNumber}
          pageCount={pageCount}
        />
      </div>
    </>
  );
}
