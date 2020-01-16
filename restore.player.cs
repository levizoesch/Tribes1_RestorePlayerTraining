/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Krayvok - 2019
// www.Krayvok.com/
// A server side script for players to repeat routes or player positioning.
// Intended as a training tool to allow endless repetitions instead of
// re-spawn and trying to be in exact position over and over.
/////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Define default to false for Training Mode.
    $Krayvok::TrainingMode = false;

    // Serverside attachments.
    Attachment::AddAfter("displayMenuPlayerMenu", "Krayvok::displayMenuTrainingMenu");
    Attachment::AddAfter("processMenuPlayerOptionsMenu", "Krayvok::processMenuTrainingOptionsMenu");

    // add to player options menu
    function Krayvok::displayMenuTrainingMenu(%cl) {
        addLine("Turn ON Training Mode", "trainingon", !$Server::TourneyMode && !$Krayvok::TrainingMode, %cl);
        addLine("Turn OFF Training Mode", "trainingoff", !$Server::TourneyMode && $Krayvok::TrainingMode, %cl);
    }

    function Krayvok::processMenuTrainingOptionsMenu(%cl,%sel) {
        if (%sel == "trainingon") {
            $Krayvok::TrainingMode = true;
            %cl.trainingmode = True;
        }
        else if (%sel == "trainingoff") {
            $Krayvok::TrainingMode = False;
            %cl.trainingmode = False;
        }
    }

    // Serverside remotes for clients.
    function remoteStorePosition(%cl) {
        Krayvok::storePlayerPosition(%cl);
    }
    function remoteRestorePosition(%cl) {
        Krayvok::restorePlayerPosition(%cl);
    }
    function remoteRestoreHealth(%cl) {
        Krayvok::restoreFullHealth(%cl);
    }

    // Store player data
    function Krayvok::storePlayerPosition(%cl) {
        if ($Krayvok::TrainingMode && !$Server::TourneyMode) {
            %clientId = %cl;
            %player = CLIENT::getOwnedObject(%cl);
            %clientId.position = GAMEBASE::getPosition(%cl);
            %clientId.rotation = GAMEBASE::getRotation(%cl);
            %clientId.velocity = ITEM::getVelocity(%cl);
            %clientId.health = GAMEBASE::getDamageLevel(%player);
            %clientId.energy = GAMEBASE::getEnergy(%player);
            $Krayvok::storedPosition = true;
            Krayvok::msgStoredPosition(%cl);
        }
        else {
            Krayvok::disabledTraining(%cl);
        }
    }

    // Restore player data
    function Krayvok::restorePlayerPosition(%cl) {
        if ($Krayvok::TrainingMode && !$Server::TourneyMode) {
            %player = CLIENT::getOwnedObject(%cl);
            GAMEBASE::setPosition(%cl, %cl.position);
            GAMEBASE::setRotation(%cl, %cl.rotation);
            ITEM::setVelocity(%cl, %cl.velocity);
            Krayvok::restoreFullHealth(%cl);
            Krayvok::restoreAmmo(%cl);
            GAMEBASE::setEnergy(%player, %cl.energy);
            Krayvok::msgRestoredPosition(%cl);
        }
        else {
            Krayvok::disabledTraining(%cl);
        }
    }

    // Restore player health
    function Krayvok::restoreFullHealth(%cl) {
        if ($Krayvok::TrainingMode && !$Server::TourneyMode) {
            %playerid = client::getOwnedObject(%cl);
            GAMEBASE::setDamageLevel(%playerid,0);
            Krayvok::msgHealthRestored(%cl);
        }
        else {
            Krayvok::disabledTraining(%cl);
        }
    }

    function Krayvok::restoreAmmo(%cl) {
        if ($Krayvok::TrainingMode && !$Server::TourneyMode) {
            Player::setItemCount(%cl,DiscLauncher,1);
            Player::setItemCount(%cl,GrenadeLauncher,1);
            Player::setItemCount(%cl,Chaingun,1);

            Player::setItemCount(%cl,DiscAmmo,200);
            Player::setItemCount(%cl,GrenadeAmmo,200);
            Player::setItemCount(%cl,BulletAmmo,200);

            Player::SetItemCount(%cl,Grenade,5);
            Player::SetItemCount(%cl,RepairKit,1);
        }
        else {
            Krayvok::disabledTraining(%cl);
        }
    }

    // Client notifications from server.
    function Krayvok::disabledTraining(%cl) {
        if (!$Server::TourneyMode) {
            Client::sendMessage(%cl, 1, "Training mode is disabled, please see the tab menu -> Player Options. ~wmine_act.wav");
        }
        else {
            Client::sendMessage(%cl, 1, "Training mode is disabled when in tournament mode. ~wmine_act.wav");
        }
    }

    function Krayvok::errorNotStored(%cl) {
        Client::sendMessage(%cl, 1, "You must store a position first before you can restore. ~wmine_act.wav");
    }

    function Krayvok::msgRestoredPosition(%cl) {
        Client::sendMessage(%cl, 1, "You have restored your player position. ~wmine_act.wav");
    }

    function Krayvok::msgStoredPosition(%cl) {
        Client::sendMessage(%cl, 1, "You have stored your player position. ~wmine_act.wav");
    }