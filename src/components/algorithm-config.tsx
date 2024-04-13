'use client';

import { useState } from 'react';
import type { Algorithm, AlgorithmType } from '@prisma/client';

import { env } from '~/lib/env';
import type { CreateAlgorithmSchema } from '~/lib/schemas/algorithms.schema';
import { Icons } from '~/components/icons';

interface Props {
  type: AlgorithmType;
  algorithm: Algorithm | undefined;
}

export const AlgorithmConfig = ({ type, algorithm }: Props) => {
  const [configureLoading, setConfigureLoading] = useState(false);
  const [configureError, setConfigureError] = useState(false);

  const onConfigure = async () => {
    const body: CreateAlgorithmSchema = {
      type,
      randomValues: true,
    };

    setConfigureLoading(true);
    setConfigureError(false);

    try {
      await fetch(`${env.API_URL}/algorithms`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${env.API_KEY}`,
        },
        body: JSON.stringify(body),
      });

      location.reload();
    } catch (error) {
      setConfigureError(true);

      console.error(error);
    }

    setConfigureLoading(false);
  };

  if (!algorithm) {
    return (
      <>
        <p className='font-semibold text-error'>No configuration found.</p>
        <div className='card-actions justify-end'>
          <button className='btn btn-primary' onClick={() => void onConfigure()} disabled={configureLoading}>
            {configureLoading && <span className='loading loading-spinner' />}
            Configure
          </button>
        </div>

        {configureError && (
          <div role='alert' className='alert alert-error'>
            <Icons.Error />
            <span>An error occurred while configuring the algorithm.</span>
          </div>
        )}
      </>
    );
  }

  return (
    <>
      <p className='font-bold'>Data found</p>
      <div className='w-full'>
        <button className='btn btn-primary w-full'>Temp</button>
      </div>
    </>
  );
};
