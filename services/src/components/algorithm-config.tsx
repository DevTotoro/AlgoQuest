'use client';

import { useEffect, useState } from 'react';
import type { Algorithm, AlgorithmType } from '@prisma/client';

import { env } from '~/lib/env';
import { copyToClipboard } from '~/lib/utils';
import type { CreateAlgorithmSchema, UpdateAlgorithmSchema } from '~/lib/schemas/algorithms.schema';
import { ErrorIcon, SaveIcon } from '~/components/icons';

interface Props {
  type: AlgorithmType;
  algorithm: Algorithm | undefined;
}

export const AlgorithmConfig = ({ type, algorithm }: Props) => {
  const [configureLoading, setConfigureLoading] = useState(false);
  const [configureError, setConfigureError] = useState(false);

  const [saveLoading, setSaveLoading] = useState(false);
  const [saveError, setSaveError] = useState(false);

  const [copyIdText, setCopyIdText] = useState('Copy ID');

  const [isDirty, setIsDirty] = useState(false);
  const [updatedConfig, setUpdatedConfig] = useState<CreateAlgorithmSchema | undefined>(undefined);

  useEffect(() => {
    if (!updatedConfig || !algorithm) {
      setIsDirty(false);

      return;
    }

    const isEqual =
      updatedConfig.numberOfValues === algorithm.numberOfValues &&
      updatedConfig.minValue === algorithm.minValue &&
      updatedConfig.maxValue === algorithm.maxValue;

    setIsDirty(!isEqual);
  }, [updatedConfig, algorithm]);

  const onConfigure = async () => {
    const body: CreateAlgorithmSchema = {
      type,
      numberOfValues: 10,
      minValue: 3,
      maxValue: 99,
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

  const onSave = async () => {
    if (!updatedConfig || !algorithm) {
      return;
    }

    const body: UpdateAlgorithmSchema = {
      id: algorithm.id,
      type,
      numberOfValues: updatedConfig.numberOfValues,
      minValue: updatedConfig.minValue,
      maxValue: updatedConfig.maxValue,
    };

    setSaveLoading(true);
    setSaveError(false);

    try {
      await fetch(`${env.API_URL}/algorithms`, {
        method: 'PUT',
        headers: {
          Authorization: `Bearer ${env.API_KEY}`,
        },
        body: JSON.stringify(body),
      });

      location.reload();
    } catch (error) {
      setSaveError(true);

      console.error(error);
    }

    setSaveLoading(false);
  };

  const onCopyId = async () => {
    if (!algorithm) {
      return;
    }

    await copyToClipboard(algorithm.id);

    setCopyIdText('Copied!');
    setTimeout(() => {
      setCopyIdText('Copy ID');
    }, 2000);
  };

  const onChangeNumberOfValues = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!algorithm) {
      return;
    }

    const value = parseInt(event.target.value, 10);

    if (!updatedConfig) {
      setUpdatedConfig({
        type,
        minValue: algorithm.minValue,
        maxValue: algorithm.maxValue,
        numberOfValues: value,
      });

      return;
    }

    setUpdatedConfig({
      ...updatedConfig,
      numberOfValues: value,
    });
  };

  const onChangeMinValue = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!algorithm) {
      return;
    }

    const value = parseInt(event.target.value, 10);

    if (!updatedConfig) {
      setUpdatedConfig({
        type,
        minValue: value,
        maxValue: algorithm.maxValue > value ? algorithm.maxValue : value,
        numberOfValues: algorithm.numberOfValues,
      });

      return;
    }

    setUpdatedConfig({
      ...updatedConfig,
      minValue: value,
      maxValue: updatedConfig.maxValue > value ? updatedConfig.maxValue : value,
    });
  };

  const onChangeMaxValue = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!algorithm) {
      return;
    }

    const value = parseInt(event.target.value, 10);

    if (!updatedConfig) {
      setUpdatedConfig({
        type,
        minValue: algorithm.minValue < value ? algorithm.minValue : value,
        maxValue: value,
        numberOfValues: algorithm.numberOfValues,
      });

      return;
    }

    setUpdatedConfig({
      ...updatedConfig,
      maxValue: value,
      minValue: updatedConfig.minValue < value ? updatedConfig.minValue : value,
    });
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
            <ErrorIcon />
            <span>An error occurred while configuring the algorithm.</span>
          </div>
        )}
      </>
    );
  }

  return (
    <>
      <label className='form-control w-full'>
        <div className='label'>
          <span className='label-text'>Number of values</span>
          <span className='label-text-alt'>{updatedConfig?.numberOfValues ?? algorithm.numberOfValues}</span>
        </div>
        <input
          type='range'
          className='range range-sm'
          min={3}
          max={30}
          step={1}
          value={updatedConfig?.numberOfValues ?? algorithm.numberOfValues}
          onChange={onChangeNumberOfValues}
        />
      </label>

      <div className='flex flex-row space-x-4'>
        <label className='form-control w-full'>
          <div className='label'>
            <span className='label-text'>Minimum value</span>
            <span className='label-text-alt'>{updatedConfig?.minValue ?? algorithm.minValue}</span>
          </div>
          <input
            type='range'
            className='range range-sm'
            min={3}
            max={99}
            step={1}
            value={updatedConfig?.minValue ?? algorithm.minValue}
            onChange={onChangeMinValue}
          />
        </label>

        <label className='form-control w-full'>
          <div className='label'>
            <span className='label-text'>Maximum value</span>
            <span className='label-text-alt'>{updatedConfig?.maxValue ?? algorithm.maxValue}</span>
          </div>
          <input
            type='range'
            className='range range-sm'
            min={3}
            max={99}
            step={1}
            value={updatedConfig?.maxValue ?? algorithm.maxValue}
            onChange={onChangeMaxValue}
          />
        </label>
      </div>

      <div className='card-actions justify-between'>
        <button className='btn btn-secondary' onClick={() => void onCopyId()}>
          {copyIdText}
        </button>

        <div className='flex space-x-2'>
          <button
            className='btn'
            onClick={() => {
              setUpdatedConfig(undefined);
            }}
            disabled={!isDirty}
          >
            Reset
          </button>

          <button className='btn btn-primary' onClick={() => void onSave()} disabled={!isDirty || saveLoading}>
            {saveLoading ? <span className='loading loading-spinner' /> : <SaveIcon />}
            Save
          </button>
        </div>
      </div>

      {saveError && (
        <div role='alert' className='alert alert-error'>
          <ErrorIcon />
          <span>An error occurred while updating the algorithm.</span>
        </div>
      )}
    </>
  );
};
