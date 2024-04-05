-- CreateEnum
CREATE TYPE "AlgorithmType" AS ENUM ('BUBBLE_SORT', 'SELECTION_SORT');

-- CreateEnum
CREATE TYPE "CompletionStatus" AS ENUM ('SUCCESS', 'FAILURE');

-- CreateTable
CREATE TABLE "algorithms" (
    "id" TEXT NOT NULL,
    "type" "AlgorithmType" NOT NULL,
    "status" "CompletionStatus" NOT NULL,
    "time" INTEGER NOT NULL,
    "created_at" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "algorithms_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "_AlgorithmToSession" (
    "A" TEXT NOT NULL,
    "B" TEXT NOT NULL
);

-- CreateIndex
CREATE UNIQUE INDEX "_AlgorithmToSession_AB_unique" ON "_AlgorithmToSession"("A", "B");

-- CreateIndex
CREATE INDEX "_AlgorithmToSession_B_index" ON "_AlgorithmToSession"("B");

-- AddForeignKey
ALTER TABLE "_AlgorithmToSession" ADD CONSTRAINT "_AlgorithmToSession_A_fkey" FOREIGN KEY ("A") REFERENCES "algorithms"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "_AlgorithmToSession" ADD CONSTRAINT "_AlgorithmToSession_B_fkey" FOREIGN KEY ("B") REFERENCES "sessions"("id") ON DELETE CASCADE ON UPDATE CASCADE;
