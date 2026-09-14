import http from 'k6/http';
import { Counter } from 'k6/metrics';

const sentRequests = new Counter('sent_requests');
const acceptedRequests = new Counter('accepted_requests');
const rejectedRequests = new Counter('rejected_requests');

export const options = {
    scenarios: {
        rate_limit_test: {
            executor: 'shared-iterations',
            vus: 100,
            iterations: 10000,
        },
    },
};

export default function () {
    const api = Math.floor(Math.random() * 3) + 1;

    const response = http.get(`http://localhost:500${api}/api/resource`, {
        headers: {
            'x-api-key': 'test-user'
        }
    });

        // const response = http.get(`http://localhost:5001/api/resource`, {
        // headers: {
        //     'x-api-key': 'test-user'
        // }
    //   });

    sentRequests.add(1);

    if (response.status >= 200 && response.status < 300) {
        acceptedRequests.add(1);
    } else if (response.status === 429) {
        rejectedRequests.add(1);
    }
}

export function handleSummary(data) {
    const sent = data.metrics.sent_requests.values.count;
    const accepted = data.metrics.accepted_requests.values.count;
    const rejected = data.metrics.rejected_requests.values.count;

    console.log('\n===== Rate Limiter Test =====');
    console.log(`Sent:     ${sent}`);
    console.log(`Accepted: ${accepted}`);
    console.log(`Rejected: ${rejected}`);
    console.log('=============================\n');

    return {};
}