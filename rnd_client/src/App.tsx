import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { IntlProvider } from 'react-intl';
import { CustomProvider } from 'rsuite';
import ruRU from 'rsuite/locales/ru_RU';
import locales from './locales';
import Root from './pages/root';
import Dashboard from './pages/dashboard';
import Account from './pages/account';
import Error404Page from './pages/authentication/404';
import Error403Page from './pages/authentication/403';
import Error500Page from './pages/authentication/500';
import Error503Page from './pages/authentication/503';

import SignInPage from './pages/authentication/sign-in';
import SignUpPage from './pages/authentication/sign-up';
import MembersPage from './pages/tables/members';
import VirtualizedTablePage from './pages/tables/virtualized';
import FormBasicPage from './pages/forms/basic';
import FormWizardPage from './pages/forms/wizard';
import CalendarPage from './pages/calendar';
import { appNavs } from './config';

const App = () => {
  return (
    <IntlProvider locale="ru" messages={locales.ru}>
      <CustomProvider locale={ruRU}>
        <Routes>
          <Route path="/" element={<Root navs={appNavs} />}>
            <Route index element={<Dashboard />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="account" element={<Account />} />
            <Route path="table-members" element={<MembersPage />} />
            <Route path="table-virtualized" element={<VirtualizedTablePage />} />
            <Route path="error-404" element={<Error404Page />} />
            <Route path="error-403" element={<Error403Page />} />
            <Route path="error-500" element={<Error500Page />} />
            <Route path="error-503" element={<Error503Page />} />
            <Route path="sign-in" element={<SignInPage />} />
            <Route path="sign-up" element={<SignUpPage />} />
            <Route path="form-basic" element={<FormBasicPage />} />
            <Route path="form-wizard" element={<FormWizardPage />} />
            <Route path="calendar" element={<CalendarPage />} />
          </Route>
          <Route path="*" element={<Error404Page />} />
        </Routes>
      </CustomProvider>
    </IntlProvider>
  );
};

export default App;
