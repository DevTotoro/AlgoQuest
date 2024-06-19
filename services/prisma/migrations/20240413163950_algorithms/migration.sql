-- CreateEnum
CREATE TYPE "AlgorithmType" AS ENUM ('BUBBLE_SORT', 'SELECTION_SORT');

-- CreateTable
CREATE TABLE "algorithms" (
    "id" TEXT NOT NULL,
    "type" "AlgorithmType" NOT NULL,
    "number_of_values" INTEGER NOT NULL,
    "min_value" INTEGER NOT NULL,
    "max_value" INTEGER NOT NULL,
    "created_at" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(3) NOT NULL,

    CONSTRAINT "algorithms_pkey" PRIMARY KEY ("id")
);
