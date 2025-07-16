-- Create databases for each service
CREATE DATABASE aiproctoring_auth;
CREATE DATABASE aiproctoring_proctoring;
CREATE DATABASE aiproctoring_analytics;
CREATE DATABASE aiproctoring_timeseries;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE aiproctoring_auth TO aiproctoring;
GRANT ALL PRIVILEGES ON DATABASE aiproctoring_proctoring TO aiproctoring;
GRANT ALL PRIVILEGES ON DATABASE aiproctoring_analytics TO aiproctoring;
GRANT ALL PRIVILEGES ON DATABASE aiproctoring_timeseries TO aiproctoring;

-- Enable TimescaleDB extension for time-series data
\c aiproctoring_timeseries;
CREATE EXTENSION IF NOT EXISTS timescaledb;

-- Create initial tables for monitoring data
CREATE TABLE IF NOT EXISTS monitoring_data (
    id SERIAL PRIMARY KEY,
    session_id UUID NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL,
    data_type VARCHAR(50) NOT NULL,
    data JSONB NOT NULL,
    confidence DOUBLE PRECISION,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Convert to hypertable for time-series optimization
SELECT create_hypertable('monitoring_data', 'timestamp', if_not_exists => TRUE);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_monitoring_session_id ON monitoring_data (session_id);
CREATE INDEX IF NOT EXISTS idx_monitoring_data_type ON monitoring_data (data_type);
CREATE INDEX IF NOT EXISTS idx_monitoring_timestamp ON monitoring_data (timestamp DESC);

-- Create violations table
CREATE TABLE IF NOT EXISTS violations (
    id SERIAL PRIMARY KEY,
    session_id UUID NOT NULL,
    violation_type VARCHAR(100) NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL,
    confidence DOUBLE PRECISION NOT NULL,
    severity VARCHAR(20) NOT NULL,
    description TEXT,
    evidence TEXT,
    is_resolved BOOLEAN DEFAULT FALSE,
    resolved_by VARCHAR(100),
    resolved_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Convert violations to hypertable
SELECT create_hypertable('violations', 'timestamp', if_not_exists => TRUE);

-- Create indexes for violations
CREATE INDEX IF NOT EXISTS idx_violations_session_id ON violations (session_id);
CREATE INDEX IF NOT EXISTS idx_violations_type ON violations (violation_type);
CREATE INDEX IF NOT EXISTS idx_violations_severity ON violations (severity);
CREATE INDEX IF NOT EXISTS idx_violations_timestamp ON violations (timestamp DESC);