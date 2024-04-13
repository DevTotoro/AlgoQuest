-- CreateEnum
CREATE TYPE "AlgorithmType" AS ENUM ('BUBBLE_SORT', 'SELECTION_SORT');

-- CreateTable
CREATE TABLE "algorithms" (
    "id" TEXT NOT NULL,
    "type" "AlgorithmType" NOT NULL,
    "random_values" BOOLEAN NOT NULL,
    "initial_values" INTEGER[],
    "created_at" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "algorithms_pkey" PRIMARY KEY ("id")
);
