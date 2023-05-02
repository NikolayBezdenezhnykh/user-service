import http from 'k6/http';

const data =  generateArray();

export const options = {
    discardResponseBodies: true,
    scenarios: {
      createuser: {
        executor: 'ramping-arrival-rate',
        startRate: 10,
        timeUnit: '1s', // 30 iterations per minute, i.e. 1.5 RPS
        stages: [
            { target: 50, duration: '3m' }, // go from 50 to 200 iters/s in the first 30 seconds
            { target: 80, duration: '3m' }, // hold at 200 iters/s for 3.5 minutes
            { target: 70, duration: '3m' }, // hold at 200 iters/s for 3.5 minutes
            { target: 30, duration: '3m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 40, duration: '3m' }, // ramp down back to 0 iters/s over the last 30 second
          ],
        preAllocatedVUs: 50, // the size of the VU (i.e. worker) pool for this scenario
        maxVUs: 100,
        tags: { test_type: 'createuser' },
        exec: 'apicreateuser', // this scenario is executing different code than the one above!
      },
      getuser: {
        executor: 'ramping-arrival-rate',
        startRate: 10,
        timeUnit: '1s', // 30 iterations per minute, i.e. 1.5 RPS
        stages: [
            { target: 70, duration: '3m' }, // go from 50 to 200 iters/s in the first 30 seconds
            { target: 90, duration: '3m' }, // hold at 200 iters/s for 3.5 minutes
            { target: 100, duration: '2m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 80, duration: '3m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 60, duration: '1m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 80, duration: '1m' }, // ramp down back to 0 iters/s over the last 30 second
          ],
        preAllocatedVUs: 50, // the size of the VU (i.e. worker) pool for this scenario
        maxVUs: 100,
        tags: { test_type: 'getuser' },
        exec: 'apigetuser', // this scenario is executing different code than the one above!
      },
      updateuser: {
        executor: 'ramping-arrival-rate',
        startRate: 10,
        timeUnit: '1s', // 30 iterations per minute, i.e. 1.5 RPS
        stages: [
            { target: 30, duration: '3m' }, // go from 50 to 200 iters/s in the first 30 seconds
            { target: 60, duration: '3m' }, // hold at 200 iters/s for 3.5 minutes
            { target: 30, duration: '2m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 40, duration: '3m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 20, duration: '1m' }, // ramp down back to 0 iters/s over the last 30 second
            { target: 10, duration: '1m' }, // ramp down back to 0 iters/s over the last 30 second
          ],
        preAllocatedVUs: 50, // the size of the VU (i.e. worker) pool for this scenario
        maxVUs: 100,
        tags: { test_type: 'updateuser' },
        exec: 'apiupdateuser', // this scenario is executing different code than the one above!
      },      
    },
    thresholds: {
        // we can set different thresholds for the different scenarios because
        // of the extra metric tags we set!
        'http_req_duration{test_type:createuser}': ['p(99)<350'],
        'http_req_duration{test_type:updateuser}': ['p(99)<350'],
        'http_req_duration{test_type:getuser}': ['p(99)<350'],
    },
};

export function apicreateuser() {
  const url = 'http://arch.homework/user';
  const req = JSON.stringify({
    login: "testlogin",
    firstName: "Тест",
    lastName: "Тестов",
    email: "test@mail.ru",
    phone: "+70951234567"
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  http.post(url, req, params);
}

export function apigetuser() {
    const userId = data[Math.floor(Math.random() * data.length)];  
    http.get(`http://arch.homework/user/${userId}`);
}

export function apiupdateuser() {
  const userId = data[Math.floor(Math.random() * data.length)];  
  const url = `http://arch.homework/user/${userId}`;
  const req = JSON.stringify({
    firstName: "ТестТест",
    lastName: "ТестовТестов",
    email: "testUpdate@mail.ru",
    phone: "+7987654321"
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  http.put(url, req, params);
}

function generateArray() {
  const arr = new Array(2000);
  for (let i = 0; i < 2000; i++) {
    arr[i] = i + 2;
  }
  return arr;
}
