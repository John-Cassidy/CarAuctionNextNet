'use client';

import { Auction, PagedResult } from '@/types';
import React, { useEffect, useState } from 'react';

import AppPagination from '../components/AppPagination';
import AuctionCard from './AuctionCard';
import EmptyFilter from '../components/EmptyFilter';
import Filters from './Filters';
import { getData } from '../actions/auctionActions';
import qs from 'query-string';
import { shallow } from 'zustand/shallow';
import { useParamsStore } from '@/hooks/useParamsStore';

export default function Listings() {
  const [data, setData] = useState<PagedResult<Auction>>();
  const params = useParamsStore(
    (state: any) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      filterBy: state.filterBy,
      seller: state.seller,
      winner: state.winner,
    }),
    shallow
  );

  const setParams = useParamsStore((state: any) => state.setParams);
  const url = qs.stringifyUrl({ url: '', query: params });

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber });
  }

  useEffect(() => {
    getData(url).then((data: PagedResult<Auction>) => {
      setData(data);
    });
  }, [url, setData]);

  if (!data) return <h3>Loading...</h3>;

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset />
      ) : (
        <>
          <div className='grid grid-cols-4 gap-6'>
            {data.results.map((auction: Auction) => (
              <AuctionCard key={auction.id} auction={auction} />
            ))}
          </div>
          <div className='flex justify-center mt-4'>
            <AppPagination
              pageChanged={setPageNumber}
              currentPage={params.pageNumber}
              pageCount={data.pageCount}
            />
          </div>
        </>
      )}
    </>
  );
}
