-- CreateEnum
CREATE TYPE "GameMode" AS ENUM ('GUIDED', 'TIME_TRIAL');

-- CreateTable
CREATE TABLE "container_interactions" (
    "id" TEXT NOT NULL,
    "container_index" INTEGER NOT NULL,
    "received_value" INTEGER,
    "placed_value" INTEGER,
    "is_valid" BOOLEAN NOT NULL,
    "container_values" INTEGER[],
    "algorithm_type" "AlgorithmType" NOT NULL,
    "game_mode" "GameMode" NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "session_id" TEXT NOT NULL,

    CONSTRAINT "container_interactions_pkey" PRIMARY KEY ("id")
);

-- AddForeignKey
ALTER TABLE "container_interactions" ADD CONSTRAINT "container_interactions_session_id_fkey" FOREIGN KEY ("session_id") REFERENCES "sessions"("id") ON DELETE CASCADE ON UPDATE CASCADE;
