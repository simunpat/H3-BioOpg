const path = require('path');
const fs = require('fs');
const express = require('express');
const multer = require('multer');

const app = express();

const postersDir = path.join(__dirname, 'uploads', 'posters');

fs.mkdirSync(postersDir, { recursive: true });

const storage = multer.diskStorage({
    destination: (req, file, cb) => cb(null, postersDir),

    filename: (req, file, cb) => {
        const ts = Date.now();
        const safe = file.originalname.replace(/[^a-zA-Z0-9_.-]/g, '_');

        cb(null, `${ts}_${safe}`);
    },
});

const upload = multer({
    storage,
    limits: { fileSize: 5 * 1024 * 1024 },

    fileFilter: (req, file, cb) => {
        const allowed = ['image/jpeg', 'image/pjpeg', 'image/png', 'image/x-png'];

        if (allowed.includes(file.mimetype)) return cb(null, true);

        cb(new Error('Only image files are allowed'));
    },
});

app.use('/uploads', express.static(path.join(__dirname, 'uploads')));

app.post('/upload/poster', upload.single('file'), (req, res) => {
    if (!req.file) return res.status(400).json({ error: 'No file uploaded' });

    return res.json({ url: `/uploads/posters/${req.file.filename}` });
});

app.use((err, req, res, next) => {
    if (err && err.name === 'MulterError') {
        if (err.code === 'LIMIT_FILE_SIZE') {
            return res.status(413).json({ error: 'File too large (max 5MB)' });
        }

        return res.status(400).json({ error: err.message || 'Upload error' });
    }

    if (err && err.message === 'Only image files are allowed') {
        return res.status(415).json({ error: 'Unsupported media type (JPEG/PNG only)' });
    }

    return next(err);
});

app.listen(3001, () => console.log('Uploads server on http://localhost:3001'));
