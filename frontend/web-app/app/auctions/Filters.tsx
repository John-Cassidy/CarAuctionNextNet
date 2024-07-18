import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai';

import { BsFillStopCircleFill } from 'react-icons/bs';
import { Button } from 'flowbite-react';
import React from 'react';
import { useParamsStore } from '@/hooks/useParamsStore';

const pageSizeButtons = [4, 8, 12];

const orderButtons = [
  {
    label: 'Alphabetical',
    icon: AiOutlineSortAscending,
    value: 'make',
  },
  {
    label: 'End date',
    icon: AiOutlineClockCircle,
    value: 'endingSoon',
  },
  {
    label: 'Recently added',
    icon: BsFillStopCircleFill,
    value: 'new',
  },
];

export default function Filters() {
  const pageSize = useParamsStore((state: any) => state.pageSize);
  const setParams = useParamsStore((state: any) => state.setParams);
  const orderBy = useParamsStore((state: any) => state.orderBy);
  return (
    <div className='flex justify-between items-center mb-4'>
      <div>
        <span className='uppercase text-sm text-gray-500 mr-2'>Order by</span>
        <Button.Group>
          {orderButtons.map(({ label, icon: Icon, value }) => (
            <Button
              key={value}
              onClick={() => setParams({ orderBy: value })}
              color={`${orderBy === value ? 'red' : 'gray'}`}
              className='focus:ring-0'
            >
              <Icon className='mr-3 h-4 w-4' />
              {label}
            </Button>
          ))}
        </Button.Group>
      </div>

      <div>
        <span className='uppercase text-sm text-gray-500 mr-2'>Page size</span>
        <Button.Group>
          {pageSizeButtons.map((value, i) => (
            <Button
              key={i}
              onClick={() => setParams({ pageSize: value })}
              color={`${pageSize === value ? 'red' : 'gray'}`}
              className='focus:ring-0'
            >
              {value}
            </Button>
          ))}
        </Button.Group>
      </div>
    </div>
  );
}
