import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

export let errorRate = new Rate('errors');

export let options = {
    vus: 10,
    duration: '30s',
    thresholds: {
        errors: ['rate<0.1'],
        http_req_duration: ['p(95)<500'],
    },
};

const BASE_URL = 'http://localhost:5079';

export default function () {
    const payload = JSON.stringify({
        type: 'SendEmail',
        sender: 'sender@example.com',
        to: 'receiver@example.com',
        content: 'Test content from k6'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(`${BASE_URL}/api/jobs`, payload, params);

    const success = check(res, {
        'status 200 or 201': (r) => r.status === 200 || r.status === 201,
    });

    errorRate.add(!success);

    sleep(1);
}
