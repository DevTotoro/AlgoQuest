-- CreateTable
CREATE TABLE "time_trials" (
    "id" TEXT NOT NULL,
    "type" "AlgorithmType" NOT NULL,
    "time" INTEGER NOT NULL,
    "number_of_values" INTEGER NOT NULL,
    "required_moves" INTEGER NOT NULL,
    "created_at" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "time_trials_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "_SessionToTimeTrial" (
    "A" TEXT NOT NULL,
    "B" TEXT NOT NULL
);

-- CreateIndex
CREATE UNIQUE INDEX "_SessionToTimeTrial_AB_unique" ON "_SessionToTimeTrial"("A", "B");

-- CreateIndex
CREATE INDEX "_SessionToTimeTrial_B_index" ON "_SessionToTimeTrial"("B");

-- AddForeignKey
ALTER TABLE "_SessionToTimeTrial" ADD CONSTRAINT "_SessionToTimeTrial_A_fkey" FOREIGN KEY ("A") REFERENCES "sessions"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "_SessionToTimeTrial" ADD CONSTRAINT "_SessionToTimeTrial_B_fkey" FOREIGN KEY ("B") REFERENCES "time_trials"("id") ON DELETE CASCADE ON UPDATE CASCADE;
