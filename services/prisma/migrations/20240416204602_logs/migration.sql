-- CreateEnum
CREATE TYPE "LogType" AS ENUM ('HOST_CREATED', 'HOST_CLOSED', 'HOST_JOINED', 'HOST_LEFT', 'GAME_STARTED', 'GAME_OVER', 'GAME_WON', 'GAME_RESTARTED');

-- CreateTable
CREATE TABLE "logs" (
    "id" TEXT NOT NULL,
    "type" "LogType" NOT NULL,
    "message" TEXT NOT NULL,
    "algorithm_type" "AlgorithmType",
    "game_mode" "GameMode",
    "container_values" INTEGER[],
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "session_id" TEXT,

    CONSTRAINT "logs_pkey" PRIMARY KEY ("id")
);

-- AddForeignKey
ALTER TABLE "logs" ADD CONSTRAINT "logs_session_id_fkey" FOREIGN KEY ("session_id") REFERENCES "sessions"("id") ON DELETE SET NULL ON UPDATE CASCADE;
